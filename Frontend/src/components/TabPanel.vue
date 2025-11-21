<script setup lang="ts">
import { ref } from 'vue'
import StatusPanel from './StatusPanel.vue'
import InventoryPanel from './InventoryPanel.vue'

// Tab state
const activeTab = ref<'status' | 'inventory'>('status')
</script>

<template>
  <div class="h-full flex flex-col bg-rpg-panel backdrop-blur-md pixel-border">
    <!-- Tab Headers -->
    <div class="flex border-b-2 border-rpg-gold/50">
      <button
        @click="activeTab = 'status'"
        class="flex-1 px-4 py-2 text-sm font-pixel transition-all relative"
        :class="{
          'bg-black/40 text-rpg-gold border-r border-rpg-gold/30': activeTab === 'status',
          'bg-black/20 text-gray-400 hover:text-gray-200 border-r border-gray-700': activeTab !== 'status'
        }"
      >
        <span class="drop-shadow-[1px_1px_0_#000]">角色狀態</span>
        <div v-if="activeTab === 'status'"
             class="absolute bottom-0 left-0 right-0 h-0.5 bg-rpg-gold"></div>
      </button>

      <button
        @click="activeTab = 'inventory'"
        class="flex-1 px-4 py-2 text-sm font-pixel transition-all relative"
        :class="{
          'bg-black/40 text-rpg-gold': activeTab === 'inventory',
          'bg-black/20 text-gray-400 hover:text-gray-200': activeTab !== 'inventory'
        }"
      >
        <span class="drop-shadow-[1px_1px_0_#000]">物品欄</span>
        <div v-if="activeTab === 'inventory'"
             class="absolute bottom-0 left-0 right-0 h-0.5 bg-rpg-gold"></div>
      </button>
    </div>

    <!-- Tab Content -->
    <div class="flex-1 overflow-hidden">
      <Transition name="tab-fade" mode="out-in">
        <StatusPanel v-if="activeTab === 'status'" key="status" class="h-full" />
        <InventoryPanel v-else key="inventory" class="h-full" />
      </Transition>
    </div>
  </div>
</template>

<style scoped>
/* Tab transition animations */
.tab-fade-enter-active {
  transition: opacity 0.2s ease-in;
}

.tab-fade-leave-active {
  transition: opacity 0.15s ease-out;
}

.tab-fade-enter-from {
  opacity: 0;
}

.tab-fade-leave-to {
  opacity: 0;
}

/* Pixel border style */
.pixel-border {
  border: 2px solid #000;
  box-shadow:
    0 2px 0 0 #000,
    2px 0 0 0 #000,
    0 -2px 0 0 #000,
    -2px 0 0 0 #000;
}
</style>