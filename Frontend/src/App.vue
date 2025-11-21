<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useAuthStore } from './stores/auth';
import { usePlayerStore } from './stores/player';
import Login from './components/Login.vue';
import GameTerminal from './components/GameTerminal.vue';
import StatusPanel from './components/StatusPanel.vue';
import CombatView from './components/CombatView.vue';
import InventoryPanel from './components/InventoryPanel.vue';
import MiniMap from './components/MiniMap.vue';
import CharacterCreation from './components/CharacterCreation.vue';
import GameGuide from './components/GameGuide.vue';
import QuickCommands from './components/QuickCommands.vue';
import LevelUpNotification from './components/LevelUpNotification.vue';
import { gameHub } from './services/gameHub';

const authStore = useAuthStore();
const playerStore = usePlayerStore();
const commandInput = ref('');
const showCharacterCreation = ref(false);
const pendingUsername = ref('');

const connectSignalR = async () => {
  if (!authStore.isAuthenticated) return;

  try {
    await gameHub.connect('http://localhost:5000/gameHub', authStore.token);
    
    // Set up event listeners BEFORE joining the game
    gameHub.onReceiveMessage((user: string, message: string) => {
      console.log(`${user}: ${message}`);
    });

    gameHub.onPlayerData((playerData: any) => {
      console.log('Received player data:', playerData);
      playerStore.setPlayer(playerData);
    });

    gameHub.onNeedCharacterCreation((username: string) => {
      console.log('Need character creation for:', username);
      pendingUsername.value = username;
      showCharacterCreation.value = true;
    });

    gameHub.onCharacterCreated(() => {
      console.log('Character created successfully');
      showCharacterCreation.value = false;
    });

    // Now join the game - this will trigger PlayerData event or NeedCharacterCreation
    await gameHub.joinGame(authStore.username);
  } catch (err) {
    console.error('SignalR Connection Error: ', err);
  }
};

const handleCommand = async () => {
  if (!commandInput.value) return;
  await gameHub.sendCommand(commandInput.value);
  commandInput.value = '';
};

onMounted(() => {
  if (authStore.isAuthenticated) {
    connectSignalR();
  }
});

watch(() => authStore.isAuthenticated, (newValue) => {
  if (newValue) {
    connectSignalR();
  } else {
    gameHub.stop();
  }
});

const handleLogout = () => {
  authStore.logout();
  showCharacterCreation.value = false;
  pendingUsername.value = '';
};

const handleClassSelection = async (classType: number) => {
  console.log('Selected class:', classType);
  await gameHub.createCharacter(pendingUsername.value, classType);
};

const handleCancelCreation = () => {
  showCharacterCreation.value = false;
  authStore.logout();
};
</script>

<template>
  <div class="min-h-screen bg-rpg-dark text-gray-200 font-sans overflow-hidden relative perspective-1000">
    <!-- Background Image (Pixel Art Landscape) -->
    <div class="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1542751371-adc38448a05e?q=80&w=2670&auto=format&fit=crop')] bg-cover bg-center opacity-60 pointer-events-none tilt-shift scale-110"></div>
    
    <!-- Vignette & Scanlines -->
    <div class="absolute inset-0 bg-gradient-to-b from-black/80 via-transparent to-black/80 pointer-events-none"></div>
    <div class="absolute inset-0 bg-[linear-gradient(rgba(18,16,16,0)_50%,rgba(0,0,0,0.25)_50%),linear-gradient(90deg,rgba(255,0,0,0.06),rgba(0,255,0,0.02),rgba(0,0,255,0.06))] z-20 bg-[length:100%_2px,3px_100%] pointer-events-none"></div>

    <!-- Login Screen -->
    <Login v-if="!authStore.isAuthenticated" class="relative z-30" />

    <!-- Game Screen (3-Column Layout) -->
    <div v-else class="relative z-30 h-screen p-6 flex gap-6 overflow-hidden transition-transform duration-700 ease-out transform hover:scale-[1.01]">
      <!-- Left Panel: Status (20%) -->
      <div class="w-1/5 flex flex-col gap-6">
        <StatusPanel class="flex-1 rpg-card p-4" />
        <button 
          @click="handleLogout"
          class="rpg-btn w-full bg-red-900 hover:bg-red-700 text-red-100 border-2 border-black shadow-[4px_4px_0px_0px_rgba(0,0,0,1)] active:translate-y-1 active:shadow-[2px_2px_0px_0px_rgba(0,0,0,1)]"
        >
          Logout
        </button>
      </div>

      <!-- Center Panel: Main Game (50%) -->
      <div class="w-1/2 flex flex-col gap-6">
        <!-- Top: Visual Scene / Combat -->
        <div class="h-3/5 rpg-card relative overflow-hidden group border-4 border-rpg-gold/50">
          <div class="absolute inset-0 bg-black/10 group-hover:bg-transparent transition-colors duration-500"></div>
          <CombatView />
        </div>
        
        <!-- Bottom: Terminal & Input -->
        <div class="h-2/5 flex flex-col gap-3 rpg-card p-4">
          <GameTerminal class="flex-1" />
          <div class="relative">
            <span class="absolute left-3 top-1/2 -translate-y-1/2 text-rpg-gold font-pixel text-xl">></span>
            <input type="text" 
                   v-model="commandInput"
                   @keyup.enter="handleCommand"
                   placeholder="Command..." 
                   class="rpg-input pl-8" 
                   autofocus />
          </div>
        </div>
      </div>

      <!-- Right Panel: Utility (30%) -->
      <div class="w-[30%] flex flex-col gap-6">
        <!-- Top: Mini Map -->
        <div class="h-1/4 rpg-card p-2">
          <MiniMap />
        </div>

        <!-- Middle: Quick Commands -->
        <div class="h-2/5">
          <QuickCommands />
        </div>

        <!-- Bottom: Inventory -->
        <div class="flex-1 rpg-card p-4">
          <InventoryPanel />
        </div>
      </div>
    </div>

    <!-- Character Creation Modal -->
    <CharacterCreation 
      v-if="showCharacterCreation"
      @selectClass="handleClassSelection"
      @cancel="handleCancelCreation"
    />

    <!-- Game Guide -->
    <GameGuide v-if="authStore.isAuthenticated && !showCharacterCreation" />

    <!-- Level Up Notification -->
    <LevelUpNotification />
  </div>
</template>

<style>
.perspective-1000 {
  perspective: 1000px;
}
</style>

<style>
body {
  background-color: #0f172a;
  margin: 0;
  overflow: hidden;
}

/* Global scrollbar styling */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.1);
}
::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 3px;
}
::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.3);
}
</style>
