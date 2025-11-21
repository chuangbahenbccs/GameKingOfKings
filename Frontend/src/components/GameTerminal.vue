<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { gameHub } from '../services/gameHub'

const messages = ref<string[]>([])

onMounted(() => {
  gameHub.onReceiveMessage((user, message) => {
    const timestamp = new Date().toLocaleTimeString();
    messages.value.push(`[${timestamp}] ${user}: ${message}`);
    
    // Auto-scroll to bottom
    // 自動捲動到底部
    const terminal = document.getElementById('terminal-output');
    if (terminal) {
      terminal.scrollTop = terminal.scrollHeight;
    }
  });
})
</script>

<template>
  <div class="bg-black/40 font-pixel text-lg p-4 rounded h-full overflow-y-auto border border-white/5 shadow-inner" id="terminal-output">
    <div v-for="(msg, index) in messages" :key="index" class="mb-1 leading-relaxed">
      <span class="text-gray-500 text-xs mr-2">[{{ (msg.split(']')[0] || '').replace('[', '') }}]</span>
      <span :class="msg.includes('System') ? 'text-rpg-gold' : 'text-gray-300'">
        {{ msg.split(']').slice(1).join(']') }}
      </span>
    </div>
  </div>
</template>

<style scoped>
/* Custom scrollbar for terminal feel */
::-webkit-scrollbar {
  width: 6px;
}
::-webkit-scrollbar-track {
  background: rgba(0,0,0,0.3); 
}
::-webkit-scrollbar-thumb {
  background: #daa520; 
  border-radius: 2px;
}
</style>
