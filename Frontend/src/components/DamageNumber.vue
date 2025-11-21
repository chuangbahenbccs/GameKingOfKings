<script setup lang="ts">
import { ref, watch } from 'vue'

interface Props {
  damage?: number
  type?: 'damage' | 'heal' | 'critical' | 'miss'
  x?: number
  y?: number
}

const props = withDefaults(defineProps<Props>(), {
  type: 'damage',
  x: 50,
  y: 50
})

const visible = ref(false)
const currentDamage = ref(0)

watch(() => props.damage, (newDamage) => {
  if (newDamage && newDamage > 0) {
    currentDamage.value = newDamage
    visible.value = true

    // 自動隱藏
    setTimeout(() => {
      visible.value = false
    }, 1500)
  }
})
</script>

<template>
  <Transition name="damage">
    <div
      v-if="visible"
      class="damage-number"
      :class="[`damage-${type}`, { 'damage-critical': type === 'critical' }]"
      :style="`left: ${x}%; top: ${y}%;`"
    >
      <!-- HD2D風格的數字效果 -->
      <div class="damage-text-wrapper">
        <span v-if="type === 'miss'" class="damage-miss-text">MISS</span>
        <template v-else>
          <!-- 主要數字 -->
          <span class="damage-main">{{ currentDamage }}</span>
          <!-- 暴擊標記 -->
          <span v-if="type === 'critical'" class="critical-mark">!</span>
        </template>
      </div>

      <!-- HD2D風格的光暈效果 -->
      <div class="damage-glow"></div>

      <!-- 粒子效果 -->
      <div class="damage-particles">
        <span v-for="i in 5" :key="i" class="particle" :style="`--i: ${i}`"></span>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.damage-number {
  position: fixed;
  z-index: 100;
  pointer-events: none;
  transform: translate(-50%, -50%);
}

.damage-text-wrapper {
  position: relative;
  display: inline-block;
}

/* 基礎傷害數字樣式 - HD2D風格 */
.damage-main {
  font-size: 3rem;
  font-weight: 900;
  font-family: 'Impact', sans-serif;
  letter-spacing: 0.05em;
  display: inline-block;
  text-shadow:
    3px 3px 0 #000,
    -3px -3px 0 #000,
    3px -3px 0 #000,
    -3px 3px 0 #000,
    0 0 20px currentColor;
  animation: damage-pop 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
  transform-origin: center bottom;
}

/* 傷害類型顏色 */
.damage-damage .damage-main {
  color: #ff4444;
  background: linear-gradient(180deg, #ff6666 0%, #ff0000 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 10px #ff0000);
}

.damage-heal .damage-main {
  color: #44ff44;
  background: linear-gradient(180deg, #66ff66 0%, #00ff00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 10px #00ff00);
}

.damage-critical .damage-main {
  color: #ffaa00;
  font-size: 4rem;
  background: linear-gradient(180deg, #ffcc00 0%, #ff8800 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 15px #ffaa00);
  animation: critical-shake 0.5s ease-out;
}

/* 暴擊標記 */
.critical-mark {
  position: absolute;
  top: -10px;
  right: -20px;
  font-size: 2rem;
  color: #ffcc00;
  font-weight: bold;
  animation: critical-mark-bounce 0.8s ease-out;
  text-shadow:
    2px 2px 0 #000,
    -2px -2px 0 #000,
    0 0 10px #ffcc00;
}

/* MISS文字 */
.damage-miss-text {
  font-size: 2rem;
  font-weight: bold;
  color: #999;
  font-style: italic;
  animation: miss-fade 1s ease-out;
  text-shadow:
    2px 2px 0 #000,
    -2px -2px 0 #000;
}

/* HD2D光暈效果 */
.damage-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 100px;
  height: 100px;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  pointer-events: none;
  animation: glow-pulse 0.6s ease-out;
  filter: blur(20px);
}

.damage-damage .damage-glow {
  background: radial-gradient(circle, #ff0000 0%, transparent 70%);
}

.damage-heal .damage-glow {
  background: radial-gradient(circle, #00ff00 0%, transparent 70%);
}

.damage-critical .damage-glow {
  background: radial-gradient(circle, #ffaa00 0%, transparent 70%);
  animation: glow-pulse-critical 0.8s ease-out;
}

/* 粒子效果 */
.damage-particles {
  position: absolute;
  top: 50%;
  left: 50%;
  pointer-events: none;
}

.particle {
  position: absolute;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  animation: particle-fly 1s ease-out forwards;
  animation-delay: calc(var(--i) * 0.05s);
}

.damage-damage .particle {
  background: #ff4444;
  box-shadow: 0 0 4px #ff0000;
}

.damage-heal .particle {
  background: #44ff44;
  box-shadow: 0 0 4px #00ff00;
}

.damage-critical .particle {
  background: #ffaa00;
  box-shadow: 0 0 6px #ffaa00;
  width: 8px;
  height: 8px;
}

/* 動畫定義 */
@keyframes damage-pop {
  0% {
    transform: scale(0) translateY(0);
    opacity: 0;
  }
  50% {
    transform: scale(1.3) translateY(-20px);
    opacity: 1;
  }
  100% {
    transform: scale(1) translateY(-40px);
    opacity: 1;
  }
}

@keyframes critical-shake {
  0%, 100% { transform: translateX(0) scale(1); }
  10% { transform: translateX(-5px) scale(1.1); }
  20% { transform: translateX(5px) scale(1.1); }
  30% { transform: translateX(-5px) scale(1.1); }
  40% { transform: translateX(5px) scale(1.1); }
  50% { transform: translateX(0) scale(1.2); }
}

@keyframes critical-mark-bounce {
  0% {
    transform: scale(0) rotate(0deg);
    opacity: 0;
  }
  50% {
    transform: scale(1.5) rotate(15deg);
    opacity: 1;
  }
  100% {
    transform: scale(1) rotate(-5deg);
    opacity: 1;
  }
}

@keyframes miss-fade {
  0% {
    transform: translateY(0) translateX(0);
    opacity: 0;
  }
  50% {
    transform: translateY(-20px) translateX(10px);
    opacity: 1;
  }
  100% {
    transform: translateY(-40px) translateX(20px);
    opacity: 0.5;
  }
}

@keyframes glow-pulse {
  0% {
    transform: translate(-50%, -50%) scale(0);
    opacity: 1;
  }
  100% {
    transform: translate(-50%, -50%) scale(2);
    opacity: 0;
  }
}

@keyframes glow-pulse-critical {
  0% {
    transform: translate(-50%, -50%) scale(0);
    opacity: 1;
  }
  50% {
    transform: translate(-50%, -50%) scale(2.5);
    opacity: 0.8;
  }
  100% {
    transform: translate(-50%, -50%) scale(3);
    opacity: 0;
  }
}

@keyframes particle-fly {
  0% {
    transform: translate(0, 0);
    opacity: 1;
  }
  100% {
    transform:
      translateX(calc(sin(var(--i) * 72deg) * 50px))
      translateY(calc(cos(var(--i) * 72deg) * -50px));
    opacity: 0;
  }
}

/* 進入/離開過渡 */
.damage-enter-active {
  transition: all 0.3s ease-out;
}

.damage-leave-active {
  transition: all 0.5s ease-in;
}

.damage-leave-to {
  transform: translate(-50%, -100%);
  opacity: 0;
}
</style>