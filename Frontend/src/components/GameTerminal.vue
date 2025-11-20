<template>
  <div class="flex flex-col h-screen bg-[#1a1b26] text-[#a9b1d6] font-mono p-4">
    
    <!-- Login Overlay -->
    <div v-if="!hasJoined" class="fixed inset-0 bg-black/80 flex items-center justify-center z-50">
        <div class="bg-[#16161e] p-8 rounded border border-[#7aa2f7] w-96">
            <h2 class="text-2xl mb-4 text-[#7aa2f7]">Enter World</h2>
            <input 
                v-model="username" 
                @keyup.enter="joinGame"
                type="text" 
                class="w-full bg-[#1a1b26] border border-[#565f89] rounded px-4 py-2 mb-4 focus:outline-none focus:border-[#7aa2f7]"
                placeholder="Username"
                autofocus
            />
            <button @click="joinGame" class="w-full bg-[#7aa2f7] text-[#1a1b26] font-bold py-2 rounded hover:bg-[#2ac3de]">
                Join Game
            </button>
        </div>
    </div>

    <!-- Game Output -->
    <div class="flex-1 overflow-y-auto border border-[#7aa2f7] p-4 rounded mb-4 bg-black/30 shadow-inner" ref="outputContainer">
        <div v-for="(msg, index) in messages" :key="index" v-html="msg" class="mb-1"></div>
    </div>

    <!-- Input Area -->
    <div class="flex gap-2">
        <input 
            v-model="userInput" 
            @keyup.enter="sendCommand"
            type="text" 
            class="flex-1 bg-[#16161e] border border-[#7aa2f7] rounded px-4 py-2 focus:outline-none focus:ring-2 focus:ring-[#7aa2f7]"
            placeholder="Enter command..."
        />
        <button @click="sendCommand" class="bg-[#7aa2f7] text-[#1a1b26] font-bold px-6 py-2 rounded hover:bg-[#2ac3de] transition">
            Send
        </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from 'vue'
import { signalRService } from '../services/signalr'

const messages = signalRService.messages
const userInput = ref('')
const username = ref('')
const hasJoined = ref(false)
const outputContainer = ref<HTMLElement | null>(null)

onMounted(async () => {
    await signalRService.start()
})

// Auto-scroll to bottom
watch(messages.value, async () => {
    await nextTick()
    if (outputContainer.value) {
        outputContainer.value.scrollTop = outputContainer.value.scrollHeight
    }
})

const joinGame = async () => {
    if (!username.value) return
    await signalRService.joinGame(username.value)
    hasJoined.value = true
}

const sendCommand = async () => {
    if (!userInput.value) return
    await signalRService.sendCommand(userInput.value)
    userInput.value = ''
}
</script>

<style scoped>
/* Custom scrollbar for retro feel */
::-webkit-scrollbar {
  width: 8px;
}
::-webkit-scrollbar-track {
  background: #1a1b26; 
}
::-webkit-scrollbar-thumb {
  background: #565f89; 
  border-radius: 4px;
}
::-webkit-scrollbar-thumb:hover {
  background: #7aa2f7; 
}
</style>
