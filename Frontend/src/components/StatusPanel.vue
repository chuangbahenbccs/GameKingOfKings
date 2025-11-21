<script setup lang="ts">
import { usePlayerStore } from '../stores/player'

const playerStore = usePlayerStore()
</script>

<template>
  <div class="bg-gray-800 p-4 rounded-lg text-white">
    <h2 class="text-xl font-bold mb-4">Status</h2>
    
    <div v-if="playerStore.player">
      <div class="mb-2">
        <span class="font-bold">Name:</span> {{ playerStore.player.name }}
      </div>
      <div class="mb-2">
        <span class="font-bold">Level:</span> {{ playerStore.player.level }}
      </div>
      
      <!-- HP Bar -->
      <div class="mb-2">
        <div class="flex justify-between text-sm">
          <span>HP</span>
          <span>{{ playerStore.player.currentHp }} / {{ playerStore.player.maxHp }}</span>
        </div>
        <div class="w-full bg-gray-700 rounded-full h-2.5">
          <div class="bg-red-600 h-2.5 rounded-full" 
               :style="{ width: (playerStore.player.currentHp / playerStore.player.maxHp * 100) + '%' }"></div>
        </div>
      </div>

      <!-- MP Bar -->
      <div class="mb-2">
        <div class="flex justify-between text-sm">
          <span>MP</span>
          <span>{{ playerStore.player.currentMp }} / {{ playerStore.player.maxMp }}</span>
        </div>
        <div class="w-full bg-gray-700 rounded-full h-2.5">
          <div class="bg-blue-600 h-2.5 rounded-full" 
               :style="{ width: (playerStore.player.currentMp / playerStore.player.maxMp * 100) + '%' }"></div>
        </div>
      </div>
    </div>
    
    <div v-else class="text-gray-500 italic">
      Not connected...
    </div>
  </div>
</template>
