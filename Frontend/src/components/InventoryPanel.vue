<script setup lang="ts">
import { computed } from 'vue'
import { usePlayerStore } from '../stores/player'

const playerStore = usePlayerStore()

// 預設物品（目前還沒有物品系統，先用範例）
const defaultItems = [
  { name: '生鏽的劍', icon: '⚔️', type: 'weapon', slot: 0 },
  { name: '生命藥水', icon: '🧪', type: 'consumable', slot: 1, quantity: 3 },
  { name: '魔力藥水', icon: '💙', type: 'consumable', slot: 2, quantity: 2 },
]

// 填充物品欄（25格）
const inventory = computed(() => {
  const items = Array(25).fill(null)
  defaultItems.forEach(item => {
    if (item.slot < 25) {
      items[item.slot] = item
    }
  })
  return items
})

// 玩家金幣
const gold = computed(() => {
  // 暫時使用等級 * 100 作為金幣數量
  const level = playerStore.player?.level || 1
  return level * 100
})

// 職業對應圖標
const classIcon = computed(() => {
  const classType = playerStore.player?.class
  switch(classType) {
    case 0: return '⚔️' // 戰士
    case 1: return '🔮' // 法師
    case 2: return '✨' // 牧師
    default: return '👤'
  }
})
</script>

<template>
  <div class="h-full flex flex-col bg-black/60 rounded-lg p-3 border border-yellow-600/30">
    <div class="flex items-center justify-between mb-3 border-b border-yellow-600/30 pb-2">
      <h3 class="text-yellow-400 text-xs font-bold uppercase tracking-widest">物品欄</h3>
      <span class="text-xl">{{ classIcon }}</span>
    </div>

    <div class="grid grid-cols-5 gap-1 flex-1 overflow-y-auto">
      <div v-for="(item, index) in inventory" :key="index"
           class="aspect-square bg-black/40 border border-gray-700 flex items-center justify-center relative group cursor-pointer hover:border-yellow-400 hover:bg-yellow-400/10 transition-all">

        <template v-if="item">
          <span class="text-2xl filter drop-shadow-[2px_2px_0_rgba(0,0,0,1)]">{{ item.icon }}</span>
          <span v-if="item.quantity" class="absolute bottom-0 right-0 text-xs text-yellow-400 bg-black px-1 rounded">
            {{ item.quantity }}
          </span>
        </template>

        <!-- 空格子顯示半透明邊框 -->
        <div v-else class="w-full h-full border border-gray-800/50"></div>

        <!-- Tooltip -->
        <div v-if="item" class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 w-32 bg-gray-900 border border-yellow-600 p-2 text-xs z-20 opacity-0 group-hover:opacity-100 transition-opacity shadow-lg pointer-events-none">
          <div class="text-yellow-400 font-bold mb-1">{{ item.name }}</div>
          <div class="text-gray-400 capitalize text-[10px]">{{ item.type }}</div>
          <div v-if="item.quantity" class="text-gray-300 text-[10px] mt-1">
            數量: {{ item.quantity }}
          </div>
        </div>
      </div>
    </div>

    <!-- 底部資訊 -->
    <div class="pt-2 border-t border-yellow-600/30 mt-2">
      <!-- 金幣顯示 -->
      <div class="flex justify-between text-xs mb-1">
        <span class="text-gray-400">金幣</span>
        <span class="text-yellow-400 font-bold">{{ gold }} 💰</span>
      </div>

      <!-- 玩家資訊 -->
      <div v-if="playerStore.player" class="flex justify-between text-xs text-gray-400">
        <span>Lv.{{ playerStore.player.level }}</span>
        <span>{{ playerStore.player.name }}</span>
      </div>
    </div>
  </div>
</template>
