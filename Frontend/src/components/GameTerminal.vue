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
  <div class="bg-gray-900 text-gray-300 font-mono p-4 rounded-lg h-96 overflow-y-auto" id="terminal-output">
    <div v-for="(msg, index) in messages" :key="index" class="mb-1">
      {{ msg }}
    </div>
  </div>
</template>

<style scoped>
/* Custom scrollbar for terminal feel */
/* 自定義捲軸以營造終端機感覺 */
::-webkit-scrollbar {
  width: 8px;
}
::-webkit-scrollbar-track {
  background: #1a1b26; 
}
::-webkit-scrollbar-thumb {
  background: #414868; 
  border-radius: 4px;
}
</style>
