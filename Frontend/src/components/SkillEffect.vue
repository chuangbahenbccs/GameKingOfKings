<script setup lang="ts">
import { ref, computed, watch } from 'vue'

interface Props {
  skillName?: string
  type?: 'damage' | 'heal' | 'buff'
}

const props = defineProps<Props>()
const showEffect = ref(false)

// HD2D風格的粒子效果
const particleCount = computed(() => {
  switch(props.type) {
    case 'damage': return 20
    case 'heal': return 15
    case 'buff': return 10
    default: return 15
  }
})

const effectColor = computed(() => {
  switch(props.skillName?.toLowerCase()) {
    case 'fireball': return 'fire'
    case 'ice storm': return 'ice'
    case 'bash': return 'physical'
    case 'heal': return 'heal'
    case 'smite': return 'holy'
    default: return 'default'
  }
})

watch(() => props.skillName, (newVal) => {
  if (newVal) {
    showEffect.value = true
    setTimeout(() => {
      showEffect.value = false
    }, 2000)
  }
})
</script>

<template>
  <Transition name="skill-effect">
    <div v-if="showEffect" class="fixed inset-0 z-50 pointer-events-none">
      <!-- 主要技能特效容器 -->
      <div class="absolute inset-0 flex items-center justify-center">

        <!-- 技能名稱顯示 (HD2D風格) -->
        <div class="absolute top-20 skill-name-display">
          <div class="skill-name-text" :class="`skill-${effectColor}`">
            {{ skillName }}
          </div>
        </div>

        <!-- 火球術特效 -->
        <template v-if="skillName === 'Fireball' || skillName === '火球術'">
          <div class="fireball-effect">
            <div class="fire-core"></div>
            <div class="fire-ring"></div>
            <div class="fire-particles">
              <span v-for="i in 12" :key="i" class="fire-particle" :style="`--i: ${i}`"></span>
            </div>
          </div>
        </template>

        <!-- 冰風暴特效 -->
        <template v-if="skillName === 'Ice Storm' || skillName === '冰風暴'">
          <div class="ice-storm-effect">
            <div class="ice-shards">
              <span v-for="i in 8" :key="i" class="ice-shard" :style="`--i: ${i}`"></span>
            </div>
            <div class="frost-ring"></div>
          </div>
        </template>

        <!-- 重擊特效 -->
        <template v-if="skillName === 'Bash' || skillName === '重擊'">
          <div class="bash-effect">
            <div class="impact-wave"></div>
            <div class="impact-lines">
              <span v-for="i in 6" :key="i" class="impact-line" :style="`--i: ${i}`"></span>
            </div>
          </div>
        </template>

        <!-- 治癒術特效 -->
        <template v-if="skillName === 'Heal' || skillName === '治癒術'">
          <div class="heal-effect">
            <div class="heal-glow"></div>
            <div class="heal-particles">
              <span v-for="i in 10" :key="i" class="heal-particle" :style="`--i: ${i}`"></span>
            </div>
            <div class="heal-cross"></div>
          </div>
        </template>

        <!-- 神聖打擊特效 -->
        <template v-if="skillName === 'Smite' || skillName === '神聖打擊'">
          <div class="smite-effect">
            <div class="holy-beam"></div>
            <div class="holy-rings">
              <span v-for="i in 3" :key="i" class="holy-ring" :style="`--i: ${i}`"></span>
            </div>
          </div>
        </template>

        <!-- HD2D風格的環境光暈 -->
        <div class="hd2d-ambient-light" :class="`ambient-${effectColor}`"></div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
/* HD2D風格基礎 */
.skill-effect-enter-active, .skill-effect-leave-active {
  transition: opacity 0.3s;
}

.skill-effect-enter-from, .skill-effect-leave-to {
  opacity: 0;
}

/* 技能名稱顯示 - HD2D風格 */
.skill-name-display {
  perspective: 1000px;
}

.skill-name-text {
  font-size: 3rem;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 0.2em;
  animation: skill-name-appear 1.5s ease-out;
  text-shadow:
    0 0 20px currentColor,
    0 0 40px currentColor,
    0 0 60px currentColor,
    2px 2px 0 #000,
    -2px -2px 0 #000,
    2px -2px 0 #000,
    -2px 2px 0 #000;
  transform-style: preserve-3d;
}

.skill-fire { color: #ff6b35; }
.skill-ice { color: #4fc3f7; }
.skill-physical { color: #ffa726; }
.skill-heal { color: #66bb6a; }
.skill-holy { color: #ffd54f; }

@keyframes skill-name-appear {
  0% {
    transform: rotateX(90deg) scale(0);
    opacity: 0;
  }
  50% {
    transform: rotateX(0deg) scale(1.2);
    opacity: 1;
  }
  100% {
    transform: rotateX(0deg) scale(1);
    opacity: 0;
  }
}

/* 火球術特效 */
.fireball-effect {
  position: relative;
  width: 300px;
  height: 300px;
  animation: fireball-spin 1s ease-out;
}

.fire-core {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 100px;
  height: 100px;
  background: radial-gradient(circle, #fff 0%, #ff6b35 30%, #ff4500 100%);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: fire-pulse 0.5s ease-out;
  box-shadow:
    0 0 60px #ff6b35,
    0 0 120px #ff4500,
    0 0 180px #ff0000;
}

.fire-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 200px;
  height: 200px;
  border: 3px solid #ff6b35;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: ring-expand 1s ease-out;
}

.fire-particle {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 10px;
  height: 10px;
  background: #ff6b35;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: particle-burst 1s ease-out;
  animation-delay: calc(var(--i) * 0.05s);
}

/* 冰風暴特效 */
.ice-storm-effect {
  position: relative;
  width: 400px;
  height: 400px;
  animation: ice-storm-rotate 2s linear;
}

.ice-shard {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 40px;
  height: 80px;
  background: linear-gradient(135deg, #e1f5fe 0%, #4fc3f7 50%, #0288d1 100%);
  clip-path: polygon(50% 0%, 0% 100%, 100% 100%);
  transform-origin: center bottom;
  animation: shard-fall 1.5s ease-out;
  animation-delay: calc(var(--i) * 0.1s);
  box-shadow: 0 0 20px #4fc3f7;
}

.frost-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 300px;
  height: 300px;
  border: 5px solid #4fc3f7;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: frost-spread 1.5s ease-out;
  box-shadow:
    0 0 30px #4fc3f7,
    inset 0 0 30px #4fc3f7;
}

/* 重擊特效 */
.bash-effect {
  position: relative;
  width: 350px;
  height: 350px;
}

.impact-wave {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 150px;
  height: 150px;
  background: radial-gradient(circle, transparent 30%, #ffa726 50%, transparent 70%);
  transform: translate(-50%, -50%);
  animation: impact-shockwave 0.8s ease-out;
}

.impact-line {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 200px;
  height: 4px;
  background: linear-gradient(90deg, transparent, #ffa726, transparent);
  transform: translate(-50%, -50%) rotate(calc(60deg * var(--i)));
  animation: impact-slash 0.6s ease-out;
}

/* 治癒術特效 */
.heal-effect {
  position: relative;
  width: 300px;
  height: 300px;
}

.heal-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, #66bb6a 0%, transparent 70%);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: heal-pulse 1.5s ease-out;
  filter: blur(20px);
}

.heal-cross {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 80px;
  height: 80px;
  transform: translate(-50%, -50%);
  animation: cross-rotate 2s linear;
}

.heal-cross::before,
.heal-cross::after {
  content: '';
  position: absolute;
  background: #66bb6a;
  box-shadow: 0 0 20px #66bb6a;
}

.heal-cross::before {
  top: 0;
  left: 50%;
  width: 20px;
  height: 80px;
  transform: translateX(-50%);
}

.heal-cross::after {
  top: 50%;
  left: 0;
  width: 80px;
  height: 20px;
  transform: translateY(-50%);
}

.heal-particle {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 8px;
  height: 8px;
  background: #66bb6a;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: heal-float 2s ease-out;
  animation-delay: calc(var(--i) * 0.1s);
}

/* 神聖打擊特效 */
.smite-effect {
  position: relative;
  width: 300px;
  height: 500px;
}

.holy-beam {
  position: absolute;
  top: 0;
  left: 50%;
  width: 60px;
  height: 100%;
  background: linear-gradient(180deg, #ffd54f 0%, #fff 50%, #ffd54f 100%);
  transform: translateX(-50%);
  animation: beam-strike 1s ease-out;
  box-shadow:
    0 0 40px #ffd54f,
    0 0 80px #fff;
}

.holy-ring {
  position: absolute;
  left: 50%;
  width: 150px;
  height: 150px;
  border: 3px solid #ffd54f;
  border-radius: 50%;
  transform: translateX(-50%);
  top: calc(30% * var(--i));
  animation: ring-pulse 1s ease-out;
  animation-delay: calc(var(--i) * 0.1s);
}

/* HD2D環境光暈 */
.hd2d-ambient-light {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 600px;
  height: 600px;
  border-radius: 50%;
  transform: translate(-50%, -50%);
  pointer-events: none;
  animation: ambient-glow 2s ease-out;
  filter: blur(60px);
  opacity: 0.4;
}

.ambient-fire { background: radial-gradient(circle, #ff6b35, transparent); }
.ambient-ice { background: radial-gradient(circle, #4fc3f7, transparent); }
.ambient-physical { background: radial-gradient(circle, #ffa726, transparent); }
.ambient-heal { background: radial-gradient(circle, #66bb6a, transparent); }
.ambient-holy { background: radial-gradient(circle, #ffd54f, transparent); }

/* 動畫定義 */
@keyframes fire-pulse {
  0% { transform: translate(-50%, -50%) scale(0); }
  50% { transform: translate(-50%, -50%) scale(1.5); }
  100% { transform: translate(-50%, -50%) scale(1); }
}

@keyframes ring-expand {
  from {
    transform: translate(-50%, -50%) scale(0);
    opacity: 1;
  }
  to {
    transform: translate(-50%, -50%) scale(2);
    opacity: 0;
  }
}

@keyframes particle-burst {
  from {
    transform: translate(-50%, -50%) translateX(0) translateY(0);
    opacity: 1;
  }
  to {
    transform: translate(-50%, -50%)
      translateX(calc(cos(var(--i) * 30deg) * 200px))
      translateY(calc(sin(var(--i) * 30deg) * 200px));
    opacity: 0;
  }
}

@keyframes shard-fall {
  0% {
    transform: translate(-50%, -200px) rotate(calc(var(--i) * 45deg));
    opacity: 0;
  }
  50% {
    opacity: 1;
  }
  100% {
    transform: translate(-50%, 100px) rotate(calc(var(--i) * 45deg + 180deg));
    opacity: 0;
  }
}

@keyframes frost-spread {
  from {
    transform: translate(-50%, -50%) scale(0.5);
    opacity: 1;
  }
  to {
    transform: translate(-50%, -50%) scale(1.5);
    opacity: 0;
  }
}

@keyframes impact-shockwave {
  from {
    transform: translate(-50%, -50%) scale(0);
    opacity: 1;
  }
  to {
    transform: translate(-50%, -50%) scale(3);
    opacity: 0;
  }
}

@keyframes impact-slash {
  from {
    transform: translate(-50%, -50%) rotate(calc(60deg * var(--i))) scaleX(0);
    opacity: 1;
  }
  to {
    transform: translate(-50%, -50%) rotate(calc(60deg * var(--i))) scaleX(1);
    opacity: 0;
  }
}

@keyframes heal-pulse {
  0% { transform: translate(-50%, -50%) scale(0); opacity: 1; }
  100% { transform: translate(-50%, -50%) scale(2); opacity: 0; }
}

@keyframes heal-float {
  from {
    transform: translate(-50%, -50%) translateY(0);
    opacity: 1;
  }
  to {
    transform: translate(-50%, -50%) translateY(-100px);
    opacity: 0;
  }
}

@keyframes cross-rotate {
  from { transform: translate(-50%, -50%) rotate(0deg); }
  to { transform: translate(-50%, -50%) rotate(360deg); }
}

@keyframes beam-strike {
  0% {
    height: 0;
    opacity: 0;
  }
  50% {
    height: 100%;
    opacity: 1;
  }
  100% {
    opacity: 0;
  }
}

@keyframes ring-pulse {
  from {
    transform: translateX(-50%) scale(0);
    opacity: 1;
  }
  to {
    transform: translateX(-50%) scale(1.5);
    opacity: 0;
  }
}

@keyframes ambient-glow {
  0% { opacity: 0; }
  50% { opacity: 0.4; }
  100% { opacity: 0; }
}

@keyframes ice-storm-rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes fireball-spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(720deg); }
}
</style>