<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useAuthStore } from './stores/auth';
import { usePlayerStore } from './stores/player';
import Login from './components/Login.vue';
import GameTerminal from './components/GameTerminal.vue';
import TabPanel from './components/TabPanel.vue';
import CombatView from './components/CombatView.vue';
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
  <div class="min-h-screen bg-rpg-dark text-gray-200 font-sans overflow-hidden relative perspective-1000 hd2d-container">
    <!-- HD2D Multi-layer Background System -->
    <div class="hd2d-background-system">
      <!-- Far Background Layer -->
      <div class="hd2d-layer hd2d-layer-far">
        <div class="absolute inset-0 bg-gradient-to-b from-indigo-900/30 to-purple-900/30"></div>
      </div>

      <!-- Mid Background Layer -->
      <div class="hd2d-layer hd2d-layer-mid">
        <div class="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1542751371-adc38448a05e?q=80&w=2670&auto=format&fit=crop')] bg-cover bg-center opacity-40"></div>
      </div>

      <!-- Near Background Layer with HD2D Effects -->
      <div class="hd2d-layer hd2d-layer-near">
        <div class="hd2d-depth-blur"></div>
        <div class="hd2d-ambient-particles"></div>
      </div>
    </div>

    <!-- HD2D Lighting Effects -->
    <div class="hd2d-lighting">
      <!-- Volumetric Light Rays -->
      <div class="hd2d-volumetric-light hd2d-light-1"></div>
      <div class="hd2d-volumetric-light hd2d-light-2"></div>

      <!-- Dynamic Shadows -->
      <div class="hd2d-dynamic-shadows"></div>
    </div>

    <!-- Vignette & HD2D Post-processing -->
    <div class="absolute inset-0 bg-gradient-to-b from-black/60 via-transparent to-black/60 pointer-events-none"></div>
    <div class="hd2d-post-process"></div>
    <div class="absolute inset-0 bg-[linear-gradient(rgba(18,16,16,0)_50%,rgba(0,0,0,0.15)_50%),linear-gradient(90deg,rgba(255,0,0,0.03),rgba(0,255,0,0.01),rgba(0,0,255,0.03))] z-20 bg-[length:100%_2px,3px_100%] pointer-events-none"></div>

    <!-- Login Screen -->
    <Login v-if="!authStore.isAuthenticated" class="relative z-30" />

    <!-- Game Screen (3-Column Layout) -->
    <div v-else class="relative z-30 h-screen p-6 flex gap-6 overflow-hidden transition-transform duration-700 ease-out transform hover:scale-[1.01]">
      <!-- Left Panel: Status & Inventory with Tabs (25%) -->
      <div class="w-1/4 flex flex-col gap-6">
        <TabPanel class="flex-1 rpg-card" />
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

      <!-- Right Panel: Utility (25%) -->
      <div class="w-1/4 flex flex-col gap-6">
        <!-- Top: Mini Map -->
        <div class="h-1/3 rpg-card p-2">
          <MiniMap />
        </div>

        <!-- Bottom: Quick Commands (expanded) -->
        <div class="flex-1">
          <QuickCommands />
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

/* HD2D Container */
.hd2d-container {
  position: relative;
  transform-style: preserve-3d;
}

/* HD2D Background System */
.hd2d-background-system {
  position: absolute;
  inset: 0;
  transform-style: preserve-3d;
}

.hd2d-layer {
  position: absolute;
  inset: 0;
  transform-origin: center;
}

.hd2d-layer-far {
  transform: translateZ(-200px) scale(1.2);
  animation: hd2d-parallax-far 30s infinite ease-in-out;
}

.hd2d-layer-mid {
  transform: translateZ(-100px) scale(1.1);
  animation: hd2d-parallax-mid 25s infinite ease-in-out;
}

.hd2d-layer-near {
  transform: translateZ(0);
}

/* Depth Blur Effect */
.hd2d-depth-blur {
  position: absolute;
  inset: 0;
  backdrop-filter: blur(0.5px);
  background: linear-gradient(
    to bottom,
    transparent 0%,
    rgba(0, 0, 20, 0.1) 50%,
    rgba(0, 0, 40, 0.2) 100%
  );
}

/* Ambient Particles */
.hd2d-ambient-particles {
  position: absolute;
  inset: 0;
  background-image:
    radial-gradient(2px 2px at 20% 30%, rgba(255, 255, 255, 0.3), transparent),
    radial-gradient(2px 2px at 60% 70%, rgba(255, 255, 200, 0.2), transparent),
    radial-gradient(1px 1px at 90% 10%, rgba(255, 200, 255, 0.2), transparent);
  background-size: 300px 300px, 400px 400px, 250px 250px;
  animation: hd2d-particles 20s linear infinite;
}

/* HD2D Lighting System */
.hd2d-lighting {
  position: absolute;
  inset: 0;
  pointer-events: none;
  mix-blend-mode: screen;
}

/* Volumetric Light Rays */
.hd2d-volumetric-light {
  position: absolute;
  background: linear-gradient(
    180deg,
    rgba(255, 220, 150, 0.15),
    transparent 40%
  );
  filter: blur(40px);
  animation: hd2d-light-sway 10s infinite ease-in-out;
}

.hd2d-light-1 {
  top: -20%;
  left: 20%;
  width: 300px;
  height: 80%;
  transform: rotate(15deg);
  animation-delay: 0s;
}

.hd2d-light-2 {
  top: -20%;
  right: 30%;
  width: 250px;
  height: 70%;
  transform: rotate(-10deg);
  animation-delay: 5s;
}

/* Dynamic Shadows */
.hd2d-dynamic-shadows {
  position: absolute;
  inset: 0;
  background: radial-gradient(
    ellipse at center bottom,
    rgba(0, 0, 0, 0.3) 0%,
    transparent 50%
  );
  animation: hd2d-shadow-pulse 8s infinite ease-in-out;
}

/* HD2D Post-processing */
.hd2d-post-process {
  position: absolute;
  inset: 0;
  pointer-events: none;
  background:
    repeating-linear-gradient(
      0deg,
      transparent,
      transparent 2px,
      rgba(0, 0, 0, 0.03) 2px,
      rgba(0, 0, 0, 0.03) 4px
    );
  mix-blend-mode: multiply;
}

/* Animations */
@keyframes hd2d-parallax-far {
  0%, 100% { transform: translateZ(-200px) scale(1.2) translateX(0); }
  50% { transform: translateZ(-200px) scale(1.2) translateX(-20px); }
}

@keyframes hd2d-parallax-mid {
  0%, 100% { transform: translateZ(-100px) scale(1.1) translateY(0); }
  50% { transform: translateZ(-100px) scale(1.1) translateY(-10px); }
}

@keyframes hd2d-particles {
  from { transform: translateY(0); }
  to { transform: translateY(-300px); }
}

@keyframes hd2d-light-sway {
  0%, 100% { transform: rotate(15deg) translateX(0); }
  50% { transform: rotate(18deg) translateX(30px); }
}

@keyframes hd2d-shadow-pulse {
  0%, 100% { opacity: 0.3; }
  50% { opacity: 0.5; }
}

/* HD2D UI Enhancements */
.rpg-card {
  backdrop-filter: blur(10px);
  background: linear-gradient(
    135deg,
    rgba(0, 0, 0, 0.7),
    rgba(20, 20, 40, 0.6)
  );
  box-shadow:
    0 8px 32px rgba(0, 0, 0, 0.5),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.rpg-card:hover {
  transform: translateY(-2px) translateZ(10px);
  box-shadow:
    0 12px 40px rgba(0, 0, 0, 0.6),
    inset 0 1px 0 rgba(255, 255, 255, 0.15);
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
