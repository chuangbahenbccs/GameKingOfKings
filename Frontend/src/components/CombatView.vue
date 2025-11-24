<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { gameHub } from '../services/gameHub'
import { usePlayerStore } from '../stores/player'
import SkillEffect from './SkillEffect.vue'
import DamageNumber from './DamageNumber.vue'
import HD2DBattleScene from './HD2DBattleScene.vue'

const playerStore = usePlayerStore()

const monster = ref<any>(null)
const combatLog = ref<any[]>([])
const isFighting = ref(false)
const currentSkill = ref<string>('')
const damageInfo = ref<{ damage: number, type: string }>({ damage: 0, type: 'damage' })

onMounted(() => {
  gameHub.onCombatUpdate((update: any) => {
    console.log('收到戰鬥更新:', update);
    // Filter for current player (Prototype limitation fix)
    // Cast to any to avoid TS error if type is incomplete
    const player = playerStore.player as any;
    // 修正：使用大寫 PlayerId 來匹配後端
    if (player && update.PlayerId && update.PlayerId.toString() !== player.id) {
      console.log('不是當前玩家的戰鬥更新，忽略');
      return;
    }

    isFighting.value = true;
    // 修正：使用大寫屬性名稱
    monster.value = {
      name: update.Monster?.Name || update.Monster?.name,
      currentHp: update.Monster?.CurrentHp || update.Monster?.currentHp,
      maxHp: update.Monster?.MaxHp || update.Monster?.maxHp,
      image: update.Monster?.Image || '🐺'
    };
    
    // Update player stats in store (修正：使用大寫屬性名)
    if (playerStore.player && update.Player) {
      playerStore.player.currentHp = update.Player.CurrentHp || update.Player.currentHp;
      playerStore.player.currentMp = update.Player.CurrentMp || update.Player.currentMp;
    }

    // Add logs (修正：使用大寫屬性名)
    const logs = update.Logs || update.logs || [];
    logs.forEach((log: any) => {
      const logText = log.Text || log.text;
      combatLog.value.push({
        text: logText,
        type: log.Type || log.type
      });

      // 檢測技能施放和傷害
      if (logText.includes('施放了')) {
        // 提取技能名稱
        const skillMatch = logText.match(/施放了\s*(.+?)[！!，,]/);
        if (skillMatch) {
          currentSkill.value = skillMatch[1];
        }
      }

      // 提取傷害數字
      const damageMatch = logText.match(/造成了?\s*(\d+)\s*點傷害/);
      const healMatch = logText.match(/恢復了?\s*(\d+)\s*點生命值/);

      if (damageMatch) {
        damageInfo.value = {
          damage: parseInt(damageMatch[1]),
          type: logText.includes('暴擊') ? 'critical' : 'damage'
        };
      } else if (healMatch) {
        damageInfo.value = {
          damage: parseInt(healMatch[1]),
          type: 'heal'
        };
      }

      // Auto-scroll
      setTimeout(() => {
        const el = document.getElementById('combat-log');
        if (el) el.scrollTop = el.scrollHeight;
      }, 50);
    });

    // Handle combat end (修正：使用大寫屬性名)
    const updateType = update.Type || update.type;
    if (updateType === 'victory' || updateType === 'defeat' || updateType === 'flee') {
      isFighting.value = false;
      setTimeout(() => {
        monster.value = null; // Clear monster after delay
        combatLog.value = []; // 清空戰鬥日誌
      }, 5000);
    }
  });
})

const sendCommand = (cmd: string) => {
  gameHub.sendCommand(cmd);
}
</script>

<template>
  <div class="flex flex-col h-full bg-gray-900/50 backdrop-blur rounded-xl border border-gray-700 overflow-hidden relative">
    <!-- 技能特效層 -->
    <SkillEffect :skill-name="currentSkill" :type="'damage'" />

    <!-- 傷害數字層 -->
    <DamageNumber
      :damage="damageInfo.damage"
      :type="damageInfo.type as any"
      :x="50"
      :y="30"
    />

    <!-- HD2D Battle Scene -->
    <div class="h-3/5 relative overflow-hidden">
      <HD2DBattleScene
        :monster="monster"
        :is-fighting="isFighting"
      />
    </div>

    <!-- Combat Controls (Overlay) -->
    <div v-if="monster" class="absolute bottom-[40%] left-0 right-0 flex flex-col items-center gap-2 z-10">
      <!-- Skill Bar -->
      <div class="flex gap-1 bg-black/60 p-1 rounded border border-gray-600 mb-2">
        <button @click="sendCommand('cast bash')" class="bg-gray-800 hover:bg-gray-700 text-red-300 px-2 py-1 rounded border border-gray-600 text-xs font-mono" title="Bash (5 MP)">
          Bash
        </button>
        <button @click="sendCommand('cast fireball')" class="bg-gray-800 hover:bg-gray-700 text-blue-300 px-2 py-1 rounded border border-gray-600 text-xs font-mono" title="Fireball (15 MP)">
          Fireball
        </button>
        <button @click="sendCommand('cast heal')" class="bg-gray-800 hover:bg-gray-700 text-green-300 px-2 py-1 rounded border border-gray-600 text-xs font-mono" title="Heal (10 MP)">
          Heal
        </button>
      </div>

      <div class="flex gap-2">
        <button @click="sendCommand('kill ' + monster.name)" class="bg-red-900/80 hover:bg-red-700 text-white px-3 py-1 rounded border border-red-500 text-xs uppercase tracking-widest backdrop-blur">
          Attack
        </button>
        <button @click="sendCommand('flee')" class="bg-yellow-900/80 hover:bg-yellow-700 text-white px-3 py-1 rounded border border-yellow-500 text-xs uppercase tracking-widest backdrop-blur">
          Flee
        </button>
      </div>
    </div>

    <!-- Combat Log / Terminal -->
    <div id="combat-log" class="h-2/5 bg-black/80 p-4 font-mono text-sm overflow-y-auto border-t border-gray-700">
      <div v-for="(log, index) in combatLog" :key="index" class="mb-1 animate-fade-in-up">
        <span v-if="log.type === 'info'" class="text-blue-300">[INFO]</span>
        <span v-else-if="log.type === 'warning'" class="text-yellow-500">[WARN]</span>
        <span v-else-if="log.type === 'damage'" class="text-red-500">[DMG]</span>
        <span v-else-if="log.type === 'victory'" class="text-green-400 font-bold">[WIN]</span>
        <span v-else-if="log.type === 'defeat'" class="text-red-600 font-bold">[DEAD]</span>
        <span v-else-if="log.type === 'flee'" class="text-yellow-400 font-bold">[FLEE]</span>
        <span class="ml-2 text-gray-300">{{ log.text }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.animate-bounce-slow {
  animation: bounce 3s infinite;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

.animate-fade-in-up {
  animation: fadeInUp 0.3s ease-out;
}

@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* 保留基礎動畫 */
</style>
