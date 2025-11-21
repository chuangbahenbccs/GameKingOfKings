<script setup lang="ts">
import { ref, watch } from 'vue'
import { usePlayerStore } from '../stores/player'

const playerStore = usePlayerStore()
const showNotification = ref(false)
const previousLevel = ref(0)
const newLevel = ref(0)

// 監聽等級變化
watch(() => playerStore.player?.level, (currentLevel, oldLevel) => {
  if (currentLevel && oldLevel && currentLevel > oldLevel) {
    previousLevel.value = oldLevel
    newLevel.value = currentLevel
    showNotification.value = true

    // 5秒後自動隱藏
    setTimeout(() => {
      showNotification.value = false
    }, 5000)
  }
})
</script>

<template>
  <Transition name="level-up">
    <div v-if="showNotification" class="fixed inset-0 z-50 pointer-events-none flex items-center justify-center">
      <!-- 背景特效 -->
      <div class="absolute inset-0 bg-gradient-radial from-yellow-500/20 via-transparent to-transparent animate-pulse"></div>

      <!-- 主要通知 -->
      <div class="relative transform scale-100 animate-bounce-in">
        <!-- 光環效果 -->
        <div class="absolute inset-0 blur-xl bg-yellow-400 opacity-50 animate-glow"></div>

        <!-- 內容 -->
        <div class="relative bg-gradient-to-b from-yellow-600 to-yellow-800 text-white px-12 py-8 rounded-lg border-4 border-yellow-400 shadow-2xl">
          <div class="text-center">
            <!-- 星星裝飾 -->
            <div class="absolute -top-6 left-1/2 transform -translate-x-1/2 text-4xl animate-spin-slow">⭐</div>

            <h1 class="text-5xl font-bold mb-2 text-yellow-100 animate-text-glow">
              LEVEL UP!
            </h1>

            <div class="text-3xl font-semibold text-yellow-200">
              等級 {{ previousLevel }} → 等級 {{ newLevel }}
            </div>

            <div class="mt-4 text-lg text-yellow-100">
              🎉 恭喜升級！屬性已提升！
            </div>

            <!-- 裝飾線 -->
            <div class="mt-4 h-1 bg-gradient-to-r from-transparent via-yellow-400 to-transparent"></div>
          </div>
        </div>

        <!-- 粒子效果 -->
        <div class="absolute -top-10 -left-10 text-3xl animate-float-up opacity-80">✨</div>
        <div class="absolute -top-10 -right-10 text-3xl animate-float-up animation-delay-200 opacity-80">🌟</div>
        <div class="absolute -bottom-10 -left-10 text-3xl animate-float-up animation-delay-400 opacity-80">💫</div>
        <div class="absolute -bottom-10 -right-10 text-3xl animate-float-up animation-delay-600 opacity-80">⚡</div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
/* 主要動畫 */
.level-up-enter-active {
  transition: all 0.5s ease-out;
}

.level-up-leave-active {
  transition: all 0.5s ease-in;
}

.level-up-enter-from {
  opacity: 0;
  transform: scale(0);
}

.level-up-leave-to {
  opacity: 0;
  transform: scale(1.5);
}

/* 彈入動畫 */
@keyframes bounce-in {
  0% {
    transform: scale(0) rotate(0deg);
    opacity: 0;
  }
  50% {
    transform: scale(1.1) rotate(180deg);
  }
  100% {
    transform: scale(1) rotate(360deg);
    opacity: 1;
  }
}

.animate-bounce-in {
  animation: bounce-in 0.8s cubic-bezier(0.68, -0.55, 0.265, 1.55);
}

/* 發光動畫 */
@keyframes glow {
  0%, 100% {
    opacity: 0.3;
    transform: scale(1);
  }
  50% {
    opacity: 0.6;
    transform: scale(1.1);
  }
}

.animate-glow {
  animation: glow 2s ease-in-out infinite;
}

/* 文字發光 */
@keyframes text-glow {
  0%, 100% {
    text-shadow: 0 0 10px rgba(255, 255, 255, 0.8),
                 0 0 20px rgba(255, 255, 0, 0.8),
                 0 0 30px rgba(255, 255, 0, 0.6);
  }
  50% {
    text-shadow: 0 0 20px rgba(255, 255, 255, 1),
                 0 0 30px rgba(255, 255, 0, 1),
                 0 0 40px rgba(255, 255, 0, 0.8);
  }
}

.animate-text-glow {
  animation: text-glow 1.5s ease-in-out infinite;
}

/* 緩慢旋轉 */
@keyframes spin-slow {
  from {
    transform: translateX(-50%) rotate(0deg);
  }
  to {
    transform: translateX(-50%) rotate(360deg);
  }
}

.animate-spin-slow {
  animation: spin-slow 3s linear infinite;
}

/* 向上飄動 */
@keyframes float-up {
  0% {
    transform: translateY(0) scale(1);
    opacity: 0;
  }
  20% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) scale(0.5);
    opacity: 0;
  }
}

.animate-float-up {
  animation: float-up 3s ease-out infinite;
}

/* 動畫延遲 */
.animation-delay-200 {
  animation-delay: 200ms;
}

.animation-delay-400 {
  animation-delay: 400ms;
}

.animation-delay-600 {
  animation-delay: 600ms;
}

/* 放射狀漸變背景 */
.bg-gradient-radial {
  background: radial-gradient(circle, var(--tw-gradient-from), var(--tw-gradient-via), var(--tw-gradient-to));
}
</style>