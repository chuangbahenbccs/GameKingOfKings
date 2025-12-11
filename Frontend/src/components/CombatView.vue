<script setup lang="ts">
import { computed } from 'vue'
import { usePlayerStore } from '../stores/player'

interface Message {
  user: string;
  content: string;
  timestamp: Date;
}

const props = defineProps<{
  messages: Message[];
}>();

const playerStore = usePlayerStore();

// Filter combat messages only
const combatLog = computed(() => {
  return props.messages
    .filter(m => m.user === 'Combat' || m.content.includes('Combat') || m.content.includes('damage') || m.content.includes('⚔️'))
    .slice(-10); // Keep last 10 combat messages
});

// Get monster HP percentage
const monsterHpPercent = computed(() => {
  if (!playerStore.combat.inCombat || playerStore.combat.monsterMaxHp === 0) return 100;
  return (playerStore.combat.monsterHp / playerStore.combat.monsterMaxHp) * 100;
});

// Get message type for styling
const getLogType = (content: string): string => {
  if (content.includes('damage') || content.includes('hit')) return 'damage';
  if (content.includes('defeated') || content.includes('LEVEL UP')) return 'success';
  if (content.includes('died') || content.includes('failed')) return 'warning';
  return 'info';
};
</script>

<template>
  <div class="flex flex-col h-full bg-gray-900/50 backdrop-blur rounded-xl border border-gray-700 overflow-hidden relative">
    <!-- Scene / Monster Area -->
    <div class="h-1/2 relative bg-gradient-to-b from-gray-800 to-gray-900 flex items-center justify-center p-4">
      <!-- Background Ambience -->
      <div class="absolute inset-0 opacity-10 bg-gradient-to-br from-purple-900 to-blue-900"></div>

      <!-- Combat Active State -->
      <div v-if="playerStore.combat.inCombat" class="relative text-center transform transition-all hover:scale-105 cursor-pointer">
        <!-- HP Bar -->
        <div class="absolute -top-8 left-1/2 transform -translate-x-1/2 w-40">
          <div class="bg-gray-700 rounded-full h-3 border border-gray-600 overflow-hidden">
            <div
              class="h-full rounded-full transition-all duration-500"
              :class="monsterHpPercent > 50 ? 'bg-green-500' : monsterHpPercent > 25 ? 'bg-yellow-500' : 'bg-red-500'"
              :style="{ width: monsterHpPercent + '%' }"
            ></div>
          </div>
          <div class="text-xs text-gray-400 mt-1">
            {{ playerStore.combat.monsterHp }} / {{ playerStore.combat.monsterMaxHp }}
          </div>
        </div>

        <!-- Monster Sprite -->
        <div class="text-8xl filter drop-shadow-[0_0_15px_rgba(255,0,0,0.3)] animate-bounce-slow">
          {{ playerStore.combat.monsterEmoji }}
        </div>

        <h3 class="text-red-400 font-bold mt-2 text-lg tracking-wider">
          {{ playerStore.combat.monsterName }}
        </h3>

        <!-- Combat indicator -->
        <div class="absolute -bottom-4 left-1/2 transform -translate-x-1/2">
          <span class="text-red-500 text-2xl animate-pulse">⚔️</span>
        </div>
      </div>

      <!-- Idle State -->
      <div v-else class="text-center">
        <div class="text-6xl opacity-30 mb-4">🌲</div>
        <p class="text-gray-500 text-sm">No enemy in sight</p>
        <p class="text-gray-600 text-xs mt-2">Use "kill &lt;monster&gt;" to start combat</p>
      </div>
    </div>

    <!-- Combat Log -->
    <div class="h-1/2 bg-black/80 p-4 font-mono text-sm overflow-y-auto border-t border-gray-700">
      <div class="text-gray-500 text-xs mb-2 uppercase tracking-wider">Combat Log</div>

      <div v-if="combatLog.length === 0" class="text-gray-600 italic text-xs">
        No combat activity yet...
      </div>

      <div
        v-for="(log, index) in combatLog"
        :key="index"
        class="mb-1 animate-fade-in-up"
      >
        <span v-if="getLogType(log.content) === 'info'" class="text-blue-300">[INFO]</span>
        <span v-else-if="getLogType(log.content) === 'warning'" class="text-yellow-500">[WARN]</span>
        <span v-else-if="getLogType(log.content) === 'damage'" class="text-red-500">[DMG]</span>
        <span v-else-if="getLogType(log.content) === 'success'" class="text-green-400">[WIN]</span>
        <span class="ml-2 text-gray-300" v-html="log.content"></span>
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

/* Style HTML content from server */
:deep(.text-yellow-400) { color: rgb(250 204 21); }
:deep(.text-green-400) { color: rgb(74 222 128); }
:deep(.text-red-400) { color: rgb(248 113 113); }
:deep(.text-blue-400) { color: rgb(96 165 250); }
:deep(.text-orange-400) { color: rgb(251 146 60); }
:deep(.text-cyan-400) { color: rgb(34 211 238); }
</style>
