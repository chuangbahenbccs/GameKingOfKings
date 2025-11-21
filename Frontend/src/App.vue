<script setup lang="ts">
import { ref } from 'vue'
import GameTerminal from './components/GameTerminal.vue'
import StatusPanel from './components/StatusPanel.vue'
import LoginView from './components/LoginView.vue'
import CombatView from './components/CombatView.vue'
import InventoryPanel from './components/InventoryPanel.vue'
import MiniMap from './components/MiniMap.vue'
import { gameHub } from './services/gameHub'
import { usePlayerStore } from './stores/player'

const playerStore = usePlayerStore()
const isLoggedIn = ref(false)

const handleLogin = async (username: string) => {
  // Start connection on login
  await gameHub.start();
  
  // Set player data (Mock for now, but using real username)
  playerStore.setPlayer({
    name: username,
    currentHp: 100,
    maxHp: 100,
    currentMp: 50,
    maxMp: 50,
    level: 1,
    exp: 0
  })

  isLoggedIn.value = true
}
</script>

<template>
  <div class="min-h-screen bg-gray-950 text-white font-sans">
    <!-- Login Screen -->
    <LoginView v-if="!isLoggedIn" @login="handleLogin" />

    <!-- Game Screen (3-Column Layout) -->
    <div v-else class="h-screen p-4 flex gap-4 overflow-hidden bg-[url('https://raw.githubusercontent.com/gist/Harry/placeholder/bg_dark.jpg')] bg-cover bg-center">
      <!-- Left Panel: Status (20%) -->
      <div class="w-1/5 flex flex-col gap-4">
        <StatusPanel class="flex-1 shadow-lg" />
      </div>

      <!-- Center Panel: Main Game (50%) -->
      <div class="w-1/2 flex flex-col gap-4">
        <!-- Top: Visual Scene / Combat -->
        <div class="h-3/5 shadow-2xl">
          <CombatView />
        </div>
        
        <!-- Bottom: Terminal & Input -->
        <div class="h-2/5 flex flex-col gap-2 bg-black/80 backdrop-blur rounded-xl border border-gray-700 p-4 shadow-lg">
          <GameTerminal class="flex-1" />
          <input type="text" 
                 placeholder="Enter command..." 
                 class="w-full bg-gray-900/50 border border-gray-600 rounded p-3 text-white focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-all font-mono" 
                 autofocus />
        </div>
      </div>

      <!-- Right Panel: Utility (30%) -->
      <div class="w-[30%] flex flex-col gap-4">
        <!-- Top: Mini Map -->
        <div class="h-1/3 shadow-lg">
          <MiniMap />
        </div>
        
        <!-- Bottom: Inventory -->
        <div class="h-2/3 shadow-lg">
          <InventoryPanel />
        </div>
      </div>
    </div>
  </div>
</template>

<style>
body {
  background-color: #0f172a;
  margin: 0;
  overflow: hidden;
}

/* Global scrollbar styling */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.1);
}
::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 3px;
}
::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.3);
}
</style>
