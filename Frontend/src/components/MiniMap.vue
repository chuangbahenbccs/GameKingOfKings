<script setup lang="ts">
import { ref, onMounted } from 'vue'
// import { usePlayerStore } from '../stores/player'
import { gameHub } from '../services/gameHub'

// const playerStore = usePlayerStore()
const currentRoom = ref('村莊廣場')
const availableExits = ref<{ [key: string]: string }>({})

// 房間配置 (保留以備未來使用)
// const roomConfigs: { [key: string]: { x: number, y: number, name: string } } = {
//   '1': { x: 100, y: 100, name: '村莊廣場' },
//   '2': { x: 100, y: 50, name: '訓練場' },
//   '3': { x: 150, y: 100, name: '村長家' },
// }

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

// 計算當前房間ID (保留以備未來使用)
// const currentRoomId = computed(() => {
//   const roomId = playerStore.player?.currentRoomId || 1
//   return roomId.toString()
// })
</script>

<template>

  <div class="h-full relative overflow-hidden rounded-lg bg-black/60 border border-yellow-600/30 shadow-inner">
    <h3 class="text-yellow-400 text-xs font-bold absolute top-2 left-2 z-10 bg-black/50 px-2 py-0.5 rounded">小地圖</h3>

    <!-- Grid Background -->
    <div class="absolute inset-0 opacity-20"
         style="background-image: linear-gradient(#334155 1px, transparent 1px), linear-gradient(90deg, #334155 1px, transparent 1px); background-size: 20px 20px;">
    </div>

    <!-- Radar Scan Effect -->
    <div class="absolute inset-0 bg-gradient-to-b from-transparent via-yellow-500/5 to-transparent opacity-30 animate-scan pointer-events-none"></div>

    <!-- Map Visualization -->
    <div class="w-full h-full flex items-center justify-center relative z-0">
      <svg width="100%" height="100%" viewBox="0 0 200 200" class="drop-shadow-[0_0_5px_rgba(255,200,0,0.3)]">
        <!-- Connections based on available exits -->
        <line v-if="availableExits.n" x1="100" y1="100" x2="100" y2="50"
              stroke="#fbbf24" stroke-width="2" stroke-dasharray="4 2" />
        <line v-if="availableExits.e" x1="100" y1="100" x2="150" y2="100"
              stroke="#fbbf24" stroke-width="2" stroke-dasharray="4 2" />
        <line v-if="availableExits.w" x1="100" y1="100" x2="50" y2="100"
              stroke="#fbbf24" stroke-width="2" stroke-dasharray="4 2" />
        <line v-if="availableExits.s" x1="100" y1="100" x2="100" y2="150"
              stroke="#fbbf24" stroke-width="2" stroke-dasharray="4 2" />

        <!-- Possible Room Nodes -->
        <g v-if="availableExits.n">
          <circle cx="100" cy="50" r="6" fill="#1e293b" stroke="#fbbf24" stroke-width="2" />
          <text x="100" y="54" text-anchor="middle" fill="#fbbf24" font-size="10">北</text>
        </g>
        <g v-if="availableExits.e">
          <circle cx="150" cy="100" r="6" fill="#1e293b" stroke="#fbbf24" stroke-width="2" />
          <text x="150" y="104" text-anchor="middle" fill="#fbbf24" font-size="10">東</text>
        </g>
        <g v-if="availableExits.w">
          <circle cx="50" cy="100" r="6" fill="#1e293b" stroke="#fbbf24" stroke-width="2" />
          <text x="50" y="104" text-anchor="middle" fill="#fbbf24" font-size="10">西</text>
        </g>
        <g v-if="availableExits.s">
          <circle cx="100" cy="150" r="6" fill="#1e293b" stroke="#fbbf24" stroke-width="2" />
          <text x="100" y="154" text-anchor="middle" fill="#fbbf24" font-size="10">南</text>
        </g>

        <!-- Current Location (Center) -->
        <circle cx="100" cy="100" r="8" fill="#fbbf24" class="animate-pulse">
          <animate attributeName="opacity" values="1;0.5;1" dur="2s" repeatCount="indefinite" />
        </circle>
        <text x="100" y="105" text-anchor="middle" fill="#000" font-size="12" font-weight="bold">你</text>

        <!-- Radar Rings -->
        <circle cx="100" cy="100" r="40" fill="none" stroke="#fbbf24" stroke-width="0.5" opacity="0.2" />
        <circle cx="100" cy="100" r="80" fill="none" stroke="#fbbf24" stroke-width="0.5" opacity="0.1" />
      </svg>
    </div>

    <div class="absolute bottom-2 right-2 text-xs text-yellow-400 bg-black/60 px-2 py-1 rounded border border-yellow-600/20">
      位置: {{ currentRoom }}
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
