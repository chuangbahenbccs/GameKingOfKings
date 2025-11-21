<script setup lang="ts">
import { ref } from 'vue';

const emit = defineEmits<{
  (e: 'selectClass', classType: number): void
  (e: 'cancel'): void
}>();

const selectedClass = ref<number | null>(null);

const classes = [
  {
    id: 0,
    name: '戰士',
    nameEn: 'Warrior',
    icon: '⚔️',
    description: '強大的近戰戰士，擁有高生命值和防禦力',
    stats: {
      hp: 150,
      mp: 30,
      str: 15,
      con: 15,
      dex: 10,
      int: 5,
      wis: 5
    },
    skills: ['Bash', 'Power Strike']
  },
  {
    id: 1,
    name: '法師',
    nameEn: 'Mage',
    icon: '🔮',
    description: '精通元素魔法的施法者，擁有強大的魔法攻擊',
    stats: {
      hp: 80,
      mp: 100,
      str: 5,
      con: 8,
      dex: 10,
      int: 15,
      wis: 10
    },
    skills: ['Fireball', 'Ice Bolt']
  },
  {
    id: 2,
    name: '牧師',
    nameEn: 'Priest',
    icon: '✨',
    description: '神聖的治療者，能夠恢復生命並支援隊友',
    stats: {
      hp: 100,
      mp: 80,
      str: 5,
      con: 12,
      dex: 8,
      int: 10,
      wis: 15
    },
    skills: ['Heal', 'Holy Light']
  }
];

const selectClass = (classId: number) => {
  selectedClass.value = classId;
};

const confirmSelection = () => {
  if (selectedClass.value !== null) {
    emit('selectClass', selectedClass.value);
  }
};
</script>

<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/90 backdrop-blur-sm p-4">
    <div class="rpg-card w-full max-w-5xl max-h-[90vh] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="bg-rpg-gold/20 border-b-2 border-rpg-gold p-6">
        <h2 class="text-3xl font-fantasy text-rpg-gold text-center drop-shadow-[2px_2px_0_#000]">
          🎮 選擇你的職業
        </h2>
        <p class="text-center text-gray-300 mt-2 font-pixel text-sm">
          選擇一個職業開始你的冒險旅程
        </p>
      </div>

      <!-- Class Selection Grid -->
      <div class="flex-1 overflow-y-auto p-6 bg-black/60">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div 
            v-for="cls in classes" 
            :key="cls.id"
            @click="selectClass(cls.id)"
            :class="[
              'rpg-card p-6 cursor-pointer transition-all duration-300 hover:scale-105',
              selectedClass === cls.id 
                ? 'border-4 border-rpg-gold shadow-[0_0_20px_rgba(218,165,32,0.5)]' 
                : 'border-2 border-gray-700 hover:border-rpg-gold/50'
            ]"
          >
            <!-- Class Icon & Name -->
            <div class="text-center mb-4">
              <div class="text-6xl mb-2">{{ cls.icon }}</div>
              <h3 class="text-2xl font-fantasy text-rpg-gold">{{ cls.name }}</h3>
              <p class="text-sm text-gray-400 font-pixel">{{ cls.nameEn }}</p>
            </div>

            <!-- Description -->
            <p class="text-sm text-gray-300 mb-4 text-center">{{ cls.description }}</p>

            <!-- Stats -->
            <div class="bg-black/40 border border-gray-700 rounded p-3 mb-3">
              <h4 class="text-xs font-pixel text-rpg-gold mb-2">初始屬性</h4>
              <div class="grid grid-cols-2 gap-2 text-xs font-pixel">
                <div class="flex justify-between">
                  <span class="text-red-400">HP:</span>
                  <span class="text-white">{{ cls.stats.hp }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-blue-400">MP:</span>
                  <span class="text-white">{{ cls.stats.mp }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-red-400">STR:</span>
                  <span class="text-white">{{ cls.stats.str }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-green-400">DEX:</span>
                  <span class="text-white">{{ cls.stats.dex }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-blue-400">INT:</span>
                  <span class="text-white">{{ cls.stats.int }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-400">WIS:</span>
                  <span class="text-white">{{ cls.stats.wis }}</span>
                </div>
                <div class="flex justify-between col-span-2">
                  <span class="text-yellow-400">CON:</span>
                  <span class="text-white">{{ cls.stats.con }}</span>
                </div>
              </div>
            </div>

            <!-- Skills -->
            <div class="bg-black/40 border border-gray-700 rounded p-3">
              <h4 class="text-xs font-pixel text-rpg-gold mb-2">初始技能</h4>
              <div class="space-y-1">
                <div 
                  v-for="skill in cls.skills" 
                  :key="skill"
                  class="text-xs text-gray-300 font-pixel"
                >
                  • {{ skill }}
                </div>
              </div>
            </div>

            <!-- Selection Indicator -->
            <div 
              v-if="selectedClass === cls.id"
              class="mt-4 text-center text-rpg-gold font-pixel text-sm animate-pulse"
            >
              ✓ 已選擇
            </div>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="bg-black/60 border-t border-gray-700 p-4 flex justify-center gap-4">
        <button 
          @click="emit('cancel')"
          class="rpg-btn bg-gray-700 hover:bg-gray-600 text-gray-200 px-6 py-2"
        >
          取消
        </button>
        <button 
          @click="confirmSelection"
          :disabled="selectedClass === null"
          :class="[
            'rpg-btn px-6 py-2',
            selectedClass !== null
              ? 'bg-rpg-gold hover:bg-yellow-500 text-black'
              : 'bg-gray-600 text-gray-400 cursor-not-allowed'
          ]"
        >
          確認選擇
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.rpg-btn {
  font-family: 'Press Start 2P', monospace;
  border-width: 2px;
  border-color: black;
  box-shadow: 4px 4px 0px 0px rgba(0,0,0,1);
  transition: all 0.2s;
}

.rpg-btn:active {
  transform: translateY(4px);
  box-shadow: 2px 2px 0px 0px rgba(0,0,0,1);
}
</style>
