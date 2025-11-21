<script setup lang="ts">
import { ref } from 'vue'

// Mock data for now
const monster = ref({
  name: 'Forest Wolf',
  hp: 80,
  maxHp: 120,
  image: '🐺' // Placeholder sprite
})

const combatLog = ref([
  { text: 'You encountered a Forest Wolf!', type: 'info' },
  { text: 'Forest Wolf howls menacingly.', type: 'warning' }
])
</script>

<template>
  <div class="flex flex-col h-full bg-gray-900/50 backdrop-blur rounded-xl border border-gray-700 overflow-hidden relative">
    <!-- Scene / Monster Area -->
    <div class="h-1/2 relative bg-gradient-to-b from-gray-800 to-gray-900 flex items-center justify-center p-4">
      <!-- Background Ambience (Fog) -->
      <div class="absolute inset-0 opacity-20 bg-[url('https://raw.githubusercontent.com/gist/Harry/placeholder/fog.png')] animate-pulse"></div>
      
      <!-- Monster Container -->
      <div class="relative text-center transform transition-all hover:scale-105 cursor-pointer">
        <!-- HP Bar -->
        <div class="absolute -top-8 left-1/2 transform -translate-x-1/2 w-32 bg-gray-700 rounded-full h-2 border border-gray-600">
          <div class="bg-red-500 h-full rounded-full transition-all duration-300" 
               :style="{ width: (monster.hp / monster.maxHp * 100) + '%' }"></div>
        </div>
        
        <!-- Monster Sprite (Emoji/Image) -->
        <div class="text-8xl filter drop-shadow-[0_0_15px_rgba(255,0,0,0.3)] animate-bounce-slow">
          {{ monster.image }}
        </div>
        
        <h3 class="text-red-400 font-bold mt-2 text-lg tracking-wider">{{ monster.name }}</h3>
      </div>
    </div>

    <!-- Combat Log / Terminal -->
    <div class="h-1/2 bg-black/80 p-4 font-mono text-sm overflow-y-auto border-t border-gray-700">
      <div v-for="(log, index) in combatLog" :key="index" class="mb-1 animate-fade-in-up">
        <span v-if="log.type === 'info'" class="text-blue-300">[INFO]</span>
        <span v-else-if="log.type === 'warning'" class="text-yellow-500">[WARN]</span>
        <span v-else-if="log.type === 'damage'" class="text-red-500">[DMG]</span>
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
</style>
