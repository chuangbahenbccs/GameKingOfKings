<script setup lang="ts">
import { ref } from 'vue';

const activeTab = ref<'movement' | 'combat' | 'skills'>('movement');
const showGuide = ref(false);

const movementCommands = [
  { cmd: 'l', desc: '查看當前位置', example: 'l' },
  { cmd: 'n', desc: '往北移動', example: 'n' },
  { cmd: 's', desc: '往南移動', example: 's' },
  { cmd: 'e', desc: '往東移動', example: 'e' },
  { cmd: 'w', desc: '往西移動', example: 'w' },
];

const combatCommands = [
  { cmd: 'kill <目標>', desc: '開始戰鬥', example: 'kill wolf' },
  { cmd: 'flee', desc: '逃跑', example: 'flee' },
];

const skills = [
  { name: 'Bash', class: '戰士', mp: 5, effect: '1.5倍傷害', cmd: 'cast bash' },
  { name: 'Fireball', class: '法師', mp: 15, effect: '2.0倍傷害', cmd: 'cast fireball' },
  { name: 'Heal', class: '牧師', mp: 10, effect: '恢復生命值', cmd: 'cast heal' },
];

const toggleGuide = () => {
  showGuide.value = !showGuide.value;
};
</script>

<template>
  <!-- Toggle Button -->
  <button 
    @click="toggleGuide"
    class="fixed bottom-6 right-6 z-50 bg-rpg-gold hover:bg-yellow-500 text-black font-pixel px-4 py-2 rounded-lg shadow-lg border-2 border-black transition-all hover:scale-105"
  >
    {{ showGuide ? '✕ 關閉' : '❓ 操作說明' }}
  </button>

  <!-- Guide Modal -->
  <div 
    v-if="showGuide"
    class="fixed inset-0 z-40 flex items-center justify-center bg-black/80 backdrop-blur-sm p-4"
    @click.self="showGuide = false"
  >
    <div class="rpg-card w-full max-w-3xl max-h-[90vh] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="bg-rpg-gold/20 border-b-2 border-rpg-gold p-4">
        <h2 class="text-2xl font-fantasy text-rpg-gold text-center drop-shadow-[2px_2px_0_#000]">
          🎮 遊戲操作指南
        </h2>
      </div>

      <!-- Tabs -->
      <div class="flex border-b border-gray-700 bg-black/40">
        <button 
          @click="activeTab = 'movement'"
          :class="[
            'flex-1 py-3 font-pixel text-sm transition-colors',
            activeTab === 'movement' 
              ? 'bg-rpg-gold/20 text-rpg-gold border-b-2 border-rpg-gold' 
              : 'text-gray-400 hover:text-gray-200'
          ]"
        >
          🗺️ 移動系統
        </button>
        <button 
          @click="activeTab = 'combat'"
          :class="[
            'flex-1 py-3 font-pixel text-sm transition-colors',
            activeTab === 'combat' 
              ? 'bg-rpg-gold/20 text-rpg-gold border-b-2 border-rpg-gold' 
              : 'text-gray-400 hover:text-gray-200'
          ]"
        >
          ⚔️ 戰鬥系統
        </button>
        <button 
          @click="activeTab = 'skills'"
          :class="[
            'flex-1 py-3 font-pixel text-sm transition-colors',
            activeTab === 'skills' 
              ? 'bg-rpg-gold/20 text-rpg-gold border-b-2 border-rpg-gold' 
              : 'text-gray-400 hover:text-gray-200'
          ]"
        >
          ✨ 技能系統
        </button>
      </div>

      <!-- Content -->
      <div class="flex-1 overflow-y-auto p-6 bg-black/60">
        <!-- Movement Tab -->
        <div v-if="activeTab === 'movement'" class="space-y-4">
          <div class="bg-gray-800/50 border border-gray-700 rounded p-4">
            <h3 class="text-rpg-gold font-pixel mb-3">📍 新手村地圖</h3>
            <pre class="text-gray-300 font-mono text-sm">
        [訓練場]
            ↑
            n
            |
    [廣場] ─e→ [長老家]
   (出生點)
            </pre>
          </div>

          <div class="bg-gray-800/50 border border-gray-700 rounded p-4">
            <h3 class="text-rpg-gold font-pixel mb-3">🎯 移動指令</h3>
            <div class="space-y-2">
              <div 
                v-for="cmd in movementCommands" 
                :key="cmd.cmd"
                class="flex items-center justify-between bg-black/40 p-3 rounded border border-gray-700 hover:border-rpg-gold/50 transition-colors"
              >
                <div class="flex-1">
                  <span class="text-rpg-gold font-pixel">{{ cmd.cmd }}</span>
                  <span class="text-gray-400 ml-3">{{ cmd.desc }}</span>
                </div>
                <code class="bg-gray-900 px-2 py-1 rounded text-green-400 font-mono text-sm">
                  {{ cmd.example }}
                </code>
              </div>
            </div>
          </div>

          <div class="bg-blue-900/20 border border-blue-500/50 rounded p-4">
            <p class="text-blue-300 text-sm font-pixel">
              💡 提示: 在下方輸入框輸入指令後按 Enter 執行
            </p>
          </div>
        </div>

        <!-- Combat Tab -->
        <div v-if="activeTab === 'combat'" class="space-y-4">
          <div class="bg-gray-800/50 border border-gray-700 rounded p-4">
            <h3 class="text-rpg-gold font-pixel mb-3">⚔️ 戰鬥指令</h3>
            <div class="space-y-2">
              <div 
                v-for="cmd in combatCommands" 
                :key="cmd.cmd"
                class="flex items-center justify-between bg-black/40 p-3 rounded border border-gray-700 hover:border-rpg-gold/50 transition-colors"
              >
                <div class="flex-1">
                  <span class="text-rpg-gold font-pixel">{{ cmd.cmd }}</span>
                  <span class="text-gray-400 ml-3">{{ cmd.desc }}</span>
                </div>
                <code class="bg-gray-900 px-2 py-1 rounded text-green-400 font-mono text-sm">
                  {{ cmd.example }}
                </code>
              </div>
            </div>
          </div>

          <div class="bg-gray-800/50 border border-gray-700 rounded p-4">
            <h3 class="text-rpg-gold font-pixel mb-3">🎮 戰鬥流程</h3>
            <ol class="space-y-2 text-gray-300">
              <li class="flex items-start">
                <span class="text-rpg-gold mr-2">1.</span>
                <span>輸入 <code class="bg-gray-900 px-2 py-1 rounded text-green-400">kill wolf</code> 開始戰鬥</span>
              </li>
              <li class="flex items-start">
                <span class="text-rpg-gold mr-2">2.</span>
                <span>角色會每 2.5 秒自動攻擊</span>
              </li>
              <li class="flex items-start">
                <span class="text-rpg-gold mr-2">3.</span>
                <span>使用技能按鈕或輸入 <code class="bg-gray-900 px-2 py-1 rounded text-green-400">cast 技能名</code></span>
              </li>
              <li class="flex items-start">
                <span class="text-rpg-gold mr-2">4.</span>
                <span>輸入 <code class="bg-gray-900 px-2 py-1 rounded text-green-400">flee</code> 逃跑</span>
              </li>
            </ol>
          </div>

          <div class="bg-red-900/20 border border-red-500/50 rounded p-4">
            <p class="text-red-300 text-sm font-pixel">
              ⚠️ 注意: 戰鬥中 HP 歸零會死亡!
            </p>
          </div>
        </div>

        <!-- Skills Tab -->
        <div v-if="activeTab === 'skills'" class="space-y-4">
          <div class="bg-gray-800/50 border border-gray-700 rounded p-4">
            <h3 class="text-rpg-gold font-pixel mb-3">✨ 可用技能</h3>
            <div class="space-y-3">
              <div 
                v-for="skill in skills" 
                :key="skill.name"
                class="bg-black/40 p-4 rounded border border-gray-700 hover:border-rpg-gold/50 transition-colors"
              >
                <div class="flex items-center justify-between mb-2">
                  <h4 class="text-rpg-gold font-pixel text-lg">{{ skill.name }}</h4>
                  <span class="bg-blue-900/50 text-blue-300 px-2 py-1 rounded text-xs">
                    {{ skill.class }}
                  </span>
                </div>
                <div class="grid grid-cols-2 gap-2 text-sm mb-2">
                  <div class="text-gray-400">
                    <span class="text-blue-400">MP:</span> {{ skill.mp }}
                  </div>
                  <div class="text-gray-400">
                    <span class="text-green-400">效果:</span> {{ skill.effect }}
                  </div>
                </div>
                <code class="block bg-gray-900 px-3 py-2 rounded text-green-400 font-mono text-sm">
                  {{ skill.cmd }}
                </code>
              </div>
            </div>
          </div>

          <div class="bg-yellow-900/20 border border-yellow-500/50 rounded p-4">
            <p class="text-yellow-300 text-sm font-pixel">
              💡 提示: 注意 MP 消耗,MP 不足時無法使用技能
            </p>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="bg-black/60 border-t border-gray-700 p-4 text-center">
        <p class="text-gray-400 text-sm font-pixel">
          按 <kbd class="bg-gray-700 px-2 py-1 rounded">ESC</kbd> 或點擊外部關閉
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped>
kbd {
  font-family: monospace;
  font-size: 0.875rem;
}

/* Smooth scroll */
.overflow-y-auto {
  scrollbar-width: thin;
  scrollbar-color: #daa520 rgba(0, 0, 0, 0.3);
}

.overflow-y-auto::-webkit-scrollbar {
  width: 6px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.3);
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: #daa520;
  border-radius: 3px;
}
</style>
