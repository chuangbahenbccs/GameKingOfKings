<script setup lang="ts">
import { ref } from 'vue'

const inventory = ref(Array(25).fill(null).map((_, i) => {
  if (i === 0) return { name: 'Rusty Sword', icon: '🗡️', type: 'weapon' }
  if (i === 1) return { name: 'Health Potion', icon: '🧪', type: 'consumable' }
  return null
}))
</script>

<template>
  <div class="h-full flex flex-col">
    <h3 class="text-rpg-gold text-xs font-fantasy uppercase tracking-widest mb-3 border-b-2 border-rpg-gold/50 border-dashed pb-1">Inventory</h3>
    
    <div class="grid grid-cols-5 gap-2 overflow-y-auto pr-1">
      <div v-for="(item, index) in inventory" :key="index" 
           class="aspect-square bg-black/60 border-2 border-gray-700 flex items-center justify-center relative group cursor-pointer hover:border-rpg-gold hover:bg-rpg-gold/20 transition-none">
        
        <span v-if="item" class="text-2xl filter drop-shadow-[2px_2px_0_rgba(0,0,0,1)]">{{ item.icon }}</span>
        
        <!-- Tooltip -->
        <div v-if="item" class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 w-32 bg-rpg-dark border-2 border-rpg-gold p-2 text-xs z-20 opacity-0 group-hover:opacity-100 transition-none shadow-[4px_4px_0_0_rgba(0,0,0,0.5)]">
          <div class="font-fantasy text-rpg-gold mb-1">{{ item.name }}</div>
          <div class="text-gray-400 font-pixel capitalize text-[10px]">{{ item.type }}</div>
        </div>
      </div>
    </div>
    
    <div class="mt-auto pt-3 border-t-2 border-rpg-gold/50 border-dashed">
      <div class="flex justify-between text-xs font-pixel">
        <span class="text-gray-400">Gold</span>
        <span class="text-rpg-gold drop-shadow-[1px_1px_0_#000]">1,250 🪙</span>
      </div>
    </div>
  </div>
</template>
