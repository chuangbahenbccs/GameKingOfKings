<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { gameHub } from '../services/gameHub'
import { usePlayerStore } from '../stores/player'

const playerStore = usePlayerStore()
const currentLocation = ref('')
const availableMonsters = ref<string[]>([])

interface QuickCommand {
  label: string
  command: string
  description?: string
  color?: string
  requiredClass?: number // 0: Warrior, 1: Mage, 2: Priest
}

const quickCommands: QuickCommand[] = [
  { label: '查看', command: 'look', description: '查看當前位置', color: 'bg-blue-600' },
  { label: '說明', command: 'help', description: '顯示指令說明', color: 'bg-green-600' },
  { label: '技能', command: 'skills', description: '查看可用技能', color: 'bg-purple-600' },
  { label: '向北', command: 'n', description: '向北移動', color: 'bg-gray-600' },
  { label: '向南', command: 's', description: '向南移動', color: 'bg-gray-600' },
  { label: '向東', command: 'e', description: '向東移動', color: 'bg-gray-600' },
  { label: '向西', command: 'w', description: '向西移動', color: 'bg-gray-600' },
  { label: '逃跑', command: 'flee', description: '逃離戰鬥', color: 'bg-yellow-600' },
]

// 所有技能指令（根據職業過濾）
const allSkillCommands: QuickCommand[] = [
  // 戰士技能
  { label: '重擊', command: 'cast bash', description: '施放重擊', color: 'bg-orange-600', requiredClass: 0 },
  { label: '嘲諷', command: 'cast taunt', description: '施放嘲諷', color: 'bg-yellow-600', requiredClass: 0 },
  { label: '鐵皮', command: 'cast iron skin', description: '施放鐵皮', color: 'bg-gray-600', requiredClass: 0 },
  // 法師技能
  { label: '火球術', command: 'cast fireball', description: '施放火球術', color: 'bg-red-700', requiredClass: 1 },
  { label: '冰風暴', command: 'cast ice storm', description: '施放冰風暴', color: 'bg-cyan-600', requiredClass: 1 },
  { label: '魔法護盾', command: 'cast mana shield', description: '施放魔法護盾', color: 'bg-purple-700', requiredClass: 1 },
  // 牧師技能
  { label: '治癒術', command: 'cast heal', description: '施放治癒術', color: 'bg-green-700', requiredClass: 2 },
  { label: '祝福', command: 'cast bless', description: '施放祝福', color: 'bg-blue-600', requiredClass: 2 },
  { label: '神聖打擊', command: 'cast smite', description: '施放神聖打擊', color: 'bg-yellow-700', requiredClass: 2 },
]

// 根據玩家職業過濾技能
const skillCommands = computed(() => {
  const playerClass = playerStore.player?.class
  if (playerClass === undefined) return []
  return allSkillCommands.filter(skill => skill.requiredClass === playerClass)
})

// 動態生成戰鬥指令（根據當前地圖的怪物）
const combatCommands = computed(() => {
  // 如果有可用怪物信息，使用它們
  if (availableMonsters.value.length > 0) {
    return availableMonsters.value.map(monster => ({
      label: `攻擊${monster}`,
      command: `kill ${monster}`,
      color: 'bg-red-600'
    }))
  }

  // 預設的戰鬥指令（根據位置）
  const defaultMonsters: Record<string, string[]> = {
    '訓練場': ['木頭人偶', '訓練假人'],
    '村莊廣場': ['大老鼠'],
    '森林': ['野狼', '野豬'],
    default: ['怪物']
  }

  const locationMonsters = defaultMonsters[currentLocation.value] || defaultMonsters.default || []
  return locationMonsters.map((monster: string) => ({
    label: `攻擊${monster}`,
    command: `kill ${monster}`,
    color: 'bg-red-600'
  }))
})

// 監聽遊戲消息，解析當前位置和可用怪物
onMounted(() => {
  // 監聽接收消息事件，解析位置信息
  gameHub.onReceiveMessage((sender: string, message: string) => {
    // 解析位置信息
    if (message.includes('你在：')) {
      const locationMatch = message.match(/你在：(.+)/)
      if (locationMatch && locationMatch[1]) {
        currentLocation.value = locationMatch[1]
      }
    }

    // 解析可見怪物
    if (message.includes('這裡有：')) {
      const monstersMatch = message.match(/這裡有：(.+)/)
      if (monstersMatch && monstersMatch[1]) {
        // 清理HTML標籤
        const cleanedMonsters = monstersMatch[1].replace(/<\/?[^>]*>/g, '')
        const monsters = cleanedMonsters
          .split(/[、，,]/)
          .map(m => m.trim())
          .filter(m => m.length > 0)
        availableMonsters.value = monsters
      }
    }

    // 如果是look命令的結果，解析位置和怪物
    if (sender === 'Game' || sender === '系統') {
      const lines = message.split('\n')
      lines.forEach(line => {
        if (line.includes('你在：')) {
          const match = line.match(/你在：(.+)/)
          if (match && match[1]) currentLocation.value = match[1]
        }
        if (line.includes('這裡有：')) {
          const match = line.match(/這裡有：(.+)/)
          if (match && match[1]) {
            // 清理HTML標籤
            const cleanedMonsters = match[1].replace(/<\/?[^>]*>/g, '')
            availableMonsters.value = cleanedMonsters
              .split(/[、，,]/)
              .map(m => m.trim())
              .filter(m => m.length > 0)
          }
        }
      })
    }
  })
})

const sendCommand = async (command: string) => {
  await gameHub.sendCommand(command)
}
</script>

<template>
  <div class="bg-black/60 backdrop-blur-sm p-4 rounded-lg border border-yellow-600/30">
    <h3 class="text-yellow-400 text-sm font-bold mb-3">快速指令</h3>

    <!-- 基本指令 -->
    <div class="grid grid-cols-4 gap-2 mb-3">
      <button
        v-for="cmd in quickCommands"
        :key="cmd.command"
        @click="sendCommand(cmd.command)"
        :class="[cmd.color || 'bg-gray-600', 'hover:opacity-80 transition-opacity']"
        class="px-2 py-1 text-white text-xs rounded shadow-md"
        :title="cmd.description"
      >
        {{ cmd.label }}
      </button>
    </div>

    <!-- 技能指令（根據職業顯示） -->
    <h4 class="text-purple-400 text-xs font-bold mb-2">
      技能指令
      <span v-if="playerStore.player" class="text-yellow-400 ml-2">
        ({{ ['戰士', '法師', '牧師'][playerStore.player.class || 0] }})
      </span>
    </h4>
    <div v-if="skillCommands.length > 0" class="grid grid-cols-3 gap-2 mb-3">
      <button
        v-for="cmd in skillCommands"
        :key="cmd.command"
        @click="sendCommand(cmd.command)"
        :class="[cmd.color || 'bg-purple-600', 'hover:opacity-80 transition-opacity']"
        class="px-2 py-1 text-white text-xs rounded shadow-md"
        :title="cmd.description"
      >
        {{ cmd.label }}
      </button>
    </div>
    <div v-else class="text-gray-400 text-xs mb-3">
      請先創建角色以查看技能
    </div>

    <!-- 戰鬥指令（根據地圖顯示） -->
    <h4 class="text-red-400 text-xs font-bold mb-2">
      戰鬥指令
      <span v-if="currentLocation" class="text-yellow-400 ml-2">
        ({{ currentLocation }})
      </span>
    </h4>
    <div v-if="combatCommands.length > 0" class="grid grid-cols-2 gap-2">
      <button
        v-for="cmd in combatCommands"
        :key="cmd.command"
        @click="sendCommand(cmd.command)"
        :class="[cmd.color || 'bg-red-600', 'hover:opacity-80 transition-opacity']"
        class="px-2 py-1 text-white text-xs rounded shadow-md"
      >
        {{ cmd.label }}
      </button>
    </div>
    <div v-else class="text-gray-400 text-xs">
      這裡沒有可攻擊的目標
    </div>

    <!-- 提示文字 -->
    <div class="mt-3 text-gray-400 text-xs">
      <p>💡 提示：點擊按鈕快速執行指令</p>
      <p>或在下方輸入框輸入指令，例如：</p>
      <ul class="ml-3 mt-1">
        <li>• kill 木頭人偶（開始戰鬥）</li>
        <li>• cast fireball（施放技能）</li>
        <li>• flee（逃離戰鬥）</li>
        <li>• skills（查看技能列表）</li>
      </ul>
      <p class="mt-2 text-yellow-400">⚡ 技能比普通攻擊更強力！</p>
    </div>
  </div>
</template>