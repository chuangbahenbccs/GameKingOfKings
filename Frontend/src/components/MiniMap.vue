<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { usePlayerStore } from '../stores/player'
import { gameHub } from '../services/gameHub'

const playerStore = usePlayerStore()
const currentRoom = ref('村莊廣場')
const availableExits = ref<{ [key: string]: string }>({})

// 房間配置
const roomConfigs: { [key: string]: { x: number, y: number, name: string } } = {
  '1': { x: 100, y: 100, name: '村莊廣場' },
  '2': { x: 100, y: 50, name: '訓練場' },
  '3': { x: 150, y: 100, name: '村長家' },
}

// 監聽遊戲消息更新地圖
onMounted(() => {
  gameHub.onReceiveMessage((_sender: string, message: string) => {
    if (message.includes('你在：')) {
      const match = message.match(/你在：(.+)/)
      if (match && match[1]) {
        currentRoom.value = match[1]
      }
    }

    // 解析可用出口
    if (message.includes('出口：')) {
      const exitMatch = message.match(/出口：(.+)/)
      if (exitMatch && exitMatch[1]) {
        const exits = exitMatch[1]
        availableExits.value = {}

        if (exits.includes('北')) availableExits.value.n = '北'
        if (exits.includes('南')) availableExits.value.s = '南'
        if (exits.includes('東')) availableExits.value.e = '東'
        if (exits.includes('西')) availableExits.value.w = '西'
      }
    }
  })
})

// 計算當前房間ID
const currentRoomId = computed(() => {
  const roomId = playerStore.player?.currentRoomId || 1
  return roomId.toString()
})

// 獲取當前房間配置
const currentRoomConfig = computed(() => {
  return roomConfigs[currentRoomId.value] || roomConfigs['1']
})
</script>

<template>

  <div class="h-full relative overflow-hidden rounded-lg bg-black/60 border border-rpg-gold/30 shadow-inner">
    <h3 class="text-rpg-gold text-[10px] font-pixel uppercase tracking-widest absolute top-2 left-2 z-10 bg-black/50 px-2 py-0.5 rounded">Radar</h3>
    
    <!-- Grid Background -->
    <div class="absolute inset-0 opacity-20" 
         style="background-image: linear-gradient(#334155 1px, transparent 1px), linear-gradient(90deg, #334155 1px, transparent 1px); background-size: 20px 20px;">
    </div>

    <!-- Radar Scan Effect -->
    <div class="absolute inset-0 bg-gradient-to-b from-transparent via-rpg-blue/5 to-transparent opacity-30 animate-scan pointer-events-none"></div>
    
    <!-- Map Visualization -->
    <div class="w-full h-full flex items-center justify-center relative z-0">
      <svg width="100%" height="100%" viewBox="0 0 200 200" class="drop-shadow-[0_0_5px_rgba(0,243,255,0.3)]">
        <!-- Connections -->
        <line x1="100" y1="100" x2="100" y2="50" stroke="#475569" stroke-width="1" stroke-dasharray="4 2" />
        <line x1="100" y1="100" x2="150" y2="100" stroke="#475569" stroke-width="1" stroke-dasharray="4 2" />
        <line x1="100" y1="100" x2="50" y2="100" stroke="#475569" stroke-width="1" stroke-dasharray="4 2" />
        
        <!-- Nodes -->
        <circle cx="100" cy="50" r="4" fill="#1e293b" stroke="#94a3b8" stroke-width="1" />
        <circle cx="150" cy="100" r="4" fill="#1e293b" stroke="#94a3b8" stroke-width="1" />
        <circle cx="50" cy="100" r="4" fill="#1e293b" stroke="#94a3b8" stroke-width="1" />
        
        <!-- Current Location (Center) -->
        <circle cx="100" cy="100" r="6" fill="#00f3ff" class="animate-pulse shadow-[0_0_10px_#00f3ff]">
          <animate attributeName="opacity" values="1;0.5;1" dur="2s" repeatCount="indefinite" />
        </circle>
        
        <!-- Radar Rings -->
        <circle cx="100" cy="100" r="40" fill="none" stroke="#00f3ff" stroke-width="0.5" opacity="0.2" />
        <circle cx="100" cy="100" r="80" fill="none" stroke="#00f3ff" stroke-width="0.5" opacity="0.1" />
      </svg>
    </div>
    
    <div class="absolute bottom-2 right-2 text-[10px] text-rpg-blue font-pixel bg-black/60 px-2 py-1 rounded border border-rpg-blue/20">
      LOC: Whispering Forest
    </div>
  </div>
</template>

<style scoped>
@keyframes scan {
  0% { transform: translateY(-100%); }
  100% { transform: translateY(100%); }
}
.animate-scan {
  animation: scan 4s linear infinite;
}
</style>
