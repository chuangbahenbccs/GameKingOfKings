<script setup lang="ts">
import { computed } from 'vue'
import { usePlayerStore, ClassType } from '../stores/player'
import CharacterAvatar from './CharacterAvatar.vue'

const playerStore = usePlayerStore()

// Helper function to get class name in Chinese
const getClassName = (classType: number) => {
  switch (classType) {
    case ClassType.Warrior: return '戰士'
    case ClassType.Mage: return '法師'
    case ClassType.Priest: return '牧師'
    default: return '未知'
  }
}

// Calculate experience progress to next level
const expProgress = computed(() => {
  if (!playerStore.player) return 0
  const expForNextLevel = playerStore.player.level * 100 // Simple formula: level * 100
  return (playerStore.player.exp / expForNextLevel) * 100
})

const expToNextLevel = computed(() => {
  if (!playerStore.player) return 0
  return playerStore.player.level * 100 - playerStore.player.exp
})
</script>

<template>
  <div class="bg-rpg-panel backdrop-blur-md pixel-border text-gray-200 h-full overflow-y-auto">
    <div v-if="playerStore.player" class="p-4 space-y-4">
      <!-- Character Header with HD2D Avatar -->
      <div class="border-b-2 border-rpg-gold/50 pb-3 border-dashed">
        <div class="flex items-center gap-3 mb-2">
          <!-- HD2D Character Avatar -->
          <CharacterAvatar
            :class-type="playerStore.player.class"
            size="medium"
            :show-effect="true"
          />
          <div class="flex-1">
            <h2 class="text-xl font-fantasy text-rpg-gold tracking-wide drop-shadow-[1px_1px_0_#000]">
              {{ playerStore.player.name }}
            </h2>
            <div class="flex items-center gap-2 mt-1">
              <span class="text-xs font-pixel text-gray-400 uppercase">LVL {{ playerStore.player.level }}</span>
              <span class="text-xs font-pixel text-blue-400">{{ getClassName(playerStore.player.class) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- HP Bar -->
      <div>
        <div class="flex justify-between text-xs mb-1 font-pixel">
          <span class="text-rpg-red">❤️ HP</span>
          <span class="text-gray-300">{{ playerStore.player.currentHp }} / {{ playerStore.player.maxHp }}</span>
        </div>
        <div class="w-full bg-black h-4 border-2 border-gray-700 p-0.5">
          <div class="bg-rpg-red h-full transition-all duration-300" 
               :style="{ width: (playerStore.player.currentHp / playerStore.player.maxHp * 100) + '%' }"></div>
        </div>
      </div>

      <!-- MP Bar -->
      <div>
        <div class="flex justify-between text-xs mb-1 font-pixel">
          <span class="text-rpg-blue">💧 MP</span>
          <span class="text-gray-300">{{ playerStore.player.currentMp }} / {{ playerStore.player.maxMp }}</span>
        </div>
        <div class="w-full bg-black h-4 border-2 border-gray-700 p-0.5">
          <div class="bg-rpg-blue h-full transition-all duration-300" 
               :style="{ width: (playerStore.player.currentMp / playerStore.player.maxMp * 100) + '%' }"></div>
        </div>
      </div>

      <!-- EXP Bar -->
      <div>
        <div class="flex justify-between text-xs mb-1 font-pixel">
          <span class="text-rpg-gold">⭐ EXP</span>
          <span class="text-gray-300">{{ expToNextLevel }} to next level</span>
        </div>
        <div class="w-full bg-black h-4 border-2 border-gray-700 p-0.5">
          <div class="bg-rpg-gold h-full transition-all duration-300" 
               :style="{ width: expProgress + '%' }"></div>
        </div>
      </div>

      <!-- Character Stats -->
      <div class="border-t border-gray-700 pt-3">
        <h3 class="text-sm font-pixel text-rpg-gold mb-2 flex items-center gap-1">
          <span>📊</span>
          <span>屬性</span>
        </h3>
        <div class="grid grid-cols-2 gap-2 text-xs font-pixel">
          <div class="bg-black/40 border border-gray-700 p-2 rounded flex justify-between">
            <span class="text-red-400">💪 STR</span>
            <span class="text-white font-bold">{{ playerStore.player.stats?.str || 0 }}</span>
          </div>
          <div class="bg-black/40 border border-gray-700 p-2 rounded flex justify-between">
            <span class="text-green-400">🎯 DEX</span>
            <span class="text-white font-bold">{{ playerStore.player.stats?.dex || 0 }}</span>
          </div>
          <div class="bg-black/40 border border-gray-700 p-2 rounded flex justify-between">
            <span class="text-blue-400">🧠 INT</span>
            <span class="text-white font-bold">{{ playerStore.player.stats?.int || 0 }}</span>
          </div>
          <div class="bg-black/40 border border-gray-700 p-2 rounded flex justify-between">
            <span class="text-purple-400">✨ WIS</span>
            <span class="text-white font-bold">{{ playerStore.player.stats?.wis || 0 }}</span>
          </div>
          <div class="bg-black/40 border border-gray-700 p-2 rounded flex justify-between col-span-2">
            <span class="text-yellow-400">🛡️ CON</span>
            <span class="text-white font-bold">{{ playerStore.player.stats?.con || 0 }}</span>
          </div>
        </div>
      </div>

      <!-- Skills Section -->
      <div class="border-t border-gray-700 pt-3">
        <h3 class="text-sm font-pixel text-rpg-gold mb-2 flex items-center gap-1">
          <span>⚔️</span>
          <span>技能</span>
        </h3>
        <div v-if="playerStore.player.skills && playerStore.player.skills.length > 0" class="space-y-2">
          <div 
            v-for="skill in playerStore.player.skills" 
            :key="skill.id"
            class="bg-black/40 border border-gray-700 p-2 rounded hover:border-rpg-gold/50 transition-colors"
          >
            <div class="flex items-center justify-between mb-1">
              <span class="text-xs font-pixel text-rpg-gold">{{ skill.name }}</span>
              <span class="text-xs text-blue-400">MP: {{ skill.manaCost }}</span>
            </div>
            <p class="text-xs text-gray-400">{{ skill.description }}</p>
          </div>
        </div>
        <div v-else class="text-xs text-gray-500 italic text-center py-2">
          尚無技能
        </div>
      </div>

      <!-- Location Info -->
      <div class="border-t border-gray-700 pt-3">
        <div class="text-xs font-pixel text-gray-400 flex items-center gap-2">
          <span>📍</span>
          <span>位置: Room {{ playerStore.player.currentRoomId }}</span>
        </div>
      </div>
    </div>
    
    <div v-else class="text-gray-500 italic font-pixel text-center py-8 animate-pulse">
      Connecting...
    </div>
  </div>
</template>

<style scoped>
/* Custom scrollbar for the panel */
.overflow-y-auto::-webkit-scrollbar {
  width: 4px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.3);
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: #daa520;
  border-radius: 2px;
}
</style>
