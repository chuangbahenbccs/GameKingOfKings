<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import { gameHub } from '../services/gameHub'

interface Message {
  timestamp: string
  user: string
  content: string
  isHtml: boolean
}

const messages = ref<Message[]>([])

// 檢測是否包含HTML標籤
const containsHtml = (text: string): boolean => {
  return /<[^>]+>/.test(text)
}

onMounted(() => {
  gameHub.onReceiveMessage(async (user, message) => {
    const timestamp = new Date().toLocaleTimeString()
    const isHtml = user === 'Game' && containsHtml(message)

    messages.value.push({
      timestamp,
      user,
      content: message,
      isHtml
    })

    // Auto-scroll to bottom
    // 自動捲動到底部
    await nextTick()
    const terminal = document.getElementById('terminal-output')
    if (terminal) {
      terminal.scrollTop = terminal.scrollHeight
    }
  })
})
</script>

<template>
  <div class="bg-black/40 font-pixel text-lg p-4 rounded h-full overflow-y-auto border border-white/5 shadow-inner" id="terminal-output">
    <div v-for="(msg, index) in messages" :key="index" class="mb-1 leading-relaxed">
      <span class="text-gray-500 text-xs mr-2">[{{ msg.timestamp }}]</span>
      <span :class="msg.user.includes('System') ? 'text-rpg-gold' : 'text-gray-300'">
        {{ msg.user }}:
      </span>
      <!-- 根據是否為HTML決定渲染方式 -->
      <span v-if="msg.isHtml" v-html="msg.content" class="inline-block ml-1"></span>
      <span v-else class="text-gray-300 ml-1">{{ msg.content }}</span>
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

/* 確保v-html內容的樣式正確顯示 */
:deep(.text-cyan-400) { color: rgb(34 211 238); }
:deep(.text-yellow-400) { color: rgb(250 204 21); }
:deep(.text-gray-400) { color: rgb(156 163 175); }
:deep(.text-gray-300) { color: rgb(209 213 219); }
:deep(.text-blue-400) { color: rgb(96 165 250); }
:deep(.text-orange-400) { color: rgb(251 146 60); }
:deep(.text-red-400) { color: rgb(248 113 113); }
:deep(.font-bold) { font-weight: bold; }
:deep(.mb-2) { margin-bottom: 0.5rem; }
:deep(.mt-2) { margin-top: 0.5rem; }
</style>