<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();

const username = ref('');
const password = ref('');
const isRegistering = ref(false);
const errorMsg = ref('');

async function handleSubmit() {
  errorMsg.value = '';
  if (!username.value || !password.value) {
    errorMsg.value = 'Please enter username and password';
    return;
  }

  try {
    if (isRegistering.value) {
      await authStore.register(username.value, password.value);
    } else {
      await authStore.login(username.value, password.value);
    }
  } catch (e: any) {
    errorMsg.value = e.message || 'An error occurred';
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-transparent perspective-1000">
    <div class="w-full max-w-md p-8 space-y-6 bg-rpg-panel pixel-border relative transform rotate-x-2 transition-transform hover:rotate-0 duration-500">
      
      <div class="text-center space-y-4 border-b-4 border-double border-rpg-gold/50 pb-4">
        <h2 class="text-4xl font-fantasy text-rpg-gold drop-shadow-[2px_2px_0_#000]">
          {{ isRegistering ? 'New Game' : 'Continue' }}
        </h2>
        <p class="text-gray-400 font-pixel text-sm uppercase tracking-widest">
          {{ isRegistering ? 'Enter your name, hero' : 'Welcome back' }}
        </p>
      </div>
      
      <form @submit.prevent="handleSubmit" class="space-y-6 pt-2">
        <div class="space-y-2">
          <label class="block text-xs font-pixel text-rpg-gold uppercase tracking-widest ml-1">Username</label>
          <input 
            v-model="username" 
            type="text" 
            class="rpg-input"
            placeholder="HERO NAME"
          />
        </div>
        
        <div class="space-y-2">
          <label class="block text-xs font-pixel text-rpg-gold uppercase tracking-widest ml-1">Password</label>
          <input 
            v-model="password" 
            type="password" 
            class="rpg-input"
            placeholder="SECRET"
          />
        </div>

        <div v-if="errorMsg" class="text-rpg-red font-pixel text-sm text-center bg-black p-2 border-2 border-rpg-red">
          {{ errorMsg }}
        </div>

        <button 
          type="submit" 
          class="rpg-btn w-full mt-4 text-lg"
        >
          {{ isRegistering ? 'Start Adventure' : 'Load Game' }}
        </button>
      </form>

      <div class="text-center text-xs font-pixel text-gray-500 pt-4">
        <button 
          @click="isRegistering = !isRegistering" 
          class="text-rpg-blue hover:text-white transition-colors uppercase tracking-widest hover:underline decoration-2 underline-offset-4"
        >
          {{ isRegistering ? '>> Back to Login' : '>> Create New Hero' }}
        </button>
      </div>
    </div>
  </div>
</template>
