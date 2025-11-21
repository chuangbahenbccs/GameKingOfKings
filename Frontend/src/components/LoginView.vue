<script setup lang="ts">
import { ref } from 'vue'

const username = ref('')
const emit = defineEmits(['login'])

const handleLogin = () => {
  if (username.value.trim()) {
    emit('login', username.value)
  }
}
</script>

<template>
  <div class="fixed inset-0 flex items-center justify-center overflow-hidden bg-gray-900">
    <!-- Animated Background -->
    <div class="absolute inset-0 z-0">
      <div class="absolute top-0 left-0 w-full h-full bg-gradient-to-br from-gray-900 via-blue-900 to-black animate-gradient"></div>
      <div class="absolute top-1/4 left-1/4 w-96 h-96 bg-purple-600/30 rounded-full mix-blend-screen filter blur-3xl animate-blob"></div>
      <div class="absolute top-1/3 right-1/4 w-96 h-96 bg-blue-600/30 rounded-full mix-blend-screen filter blur-3xl animate-blob animation-delay-2000"></div>
      <div class="absolute -bottom-8 left-1/3 w-96 h-96 bg-pink-600/30 rounded-full mix-blend-screen filter blur-3xl animate-blob animation-delay-4000"></div>
    </div>

    <!-- Login Card (Glassmorphism) -->
    <div class="relative z-10 bg-white/10 backdrop-blur-lg border border-white/20 p-8 rounded-2xl shadow-2xl w-full max-w-md transform transition-all hover:scale-[1.01]">
      <div class="text-center mb-8">
        <h1 class="text-5xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-blue-400 to-purple-400 mb-2 drop-shadow-lg">
          King of Kings
        </h1>
        <p class="text-blue-200 text-sm tracking-widest uppercase">Web MUD Adventure</p>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-6">
        <div class="relative group">
          <input 
            v-model="username"
            type="text" 
            placeholder="Enter your Hero's name..." 
            class="w-full bg-black/30 border border-gray-600 text-white px-4 py-3 rounded-lg focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/50 transition-all placeholder-gray-500 group-hover:border-gray-500"
            autofocus
          />
        </div>

        <button 
          type="submit" 
          class="w-full bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-500 hover:to-purple-500 text-white font-bold py-3 px-6 rounded-lg shadow-lg transform transition-all hover:-translate-y-1 hover:shadow-blue-500/50 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
          :disabled="!username"
        >
          Enter World
        </button>
      </form>
      
      <div class="mt-6 text-center text-gray-400 text-xs">
        <p>v0.1.0 Alpha</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -50px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

.animate-blob {
  animation: blob 7s infinite;
}

.animation-delay-2000 {
  animation-delay: 2s;
}

.animation-delay-4000 {
  animation-delay: 4s;
}

.animate-gradient {
  background-size: 200% 200%;
  animation: gradient 15s ease infinite;
}

@keyframes gradient {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}
</style>
