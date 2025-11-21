<script setup lang="ts">
import { computed } from 'vue'
import { ClassType } from '../stores/player'

interface Props {
  classType: number
  size?: 'small' | 'medium' | 'large'
  showEffect?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'medium',
  showEffect: true
})

// HD2D風格的職業頭像配置
const classAvatarConfig = computed(() => {
  switch(props.classType) {
    case ClassType.Warrior:
      return {
        name: '戰士',
        emoji: '⚔️',
        color: '#ff6b6b',
        bgGradient: 'from-red-900 to-red-700',
        borderColor: 'border-red-500',
        glowColor: 'rgba(255, 107, 107, 0.5)',
        sprite: `
          <div class="warrior-sprite">
            <div class="armor-shine"></div>
            <div class="sword-glow"></div>
            👨‍🦱
          </div>
        `,
        description: '強壯的戰士，穿著厚重的盔甲'
      }
    case ClassType.Mage:
      return {
        name: '法師',
        emoji: '🧙‍♂️',
        color: '#4fc3f7',
        bgGradient: 'from-blue-900 to-blue-700',
        borderColor: 'border-blue-500',
        glowColor: 'rgba(79, 195, 247, 0.5)',
        sprite: `
          <div class="mage-sprite">
            <div class="magic-aura"></div>
            <div class="staff-crystal"></div>
            🧙‍♂️
          </div>
        `,
        description: '神秘的法師，掌握元素之力'
      }
    case ClassType.Priest:
      return {
        name: '牧師',
        emoji: '👼',
        color: '#ffd54f',
        bgGradient: 'from-yellow-900 to-yellow-700',
        borderColor: 'border-yellow-500',
        glowColor: 'rgba(255, 213, 79, 0.5)',
        sprite: `
          <div class="priest-sprite">
            <div class="holy-light"></div>
            <div class="healing-aura"></div>
            👼
          </div>
        `,
        description: '神聖的牧師，治癒與守護的使者'
      }
    default:
      return {
        name: '冒險者',
        emoji: '👤',
        color: '#9e9e9e',
        bgGradient: 'from-gray-900 to-gray-700',
        borderColor: 'border-gray-500',
        glowColor: 'rgba(158, 158, 158, 0.5)',
        sprite: '<div>👤</div>',
        description: '普通的冒險者'
      }
  }
})

const sizeClasses = computed(() => {
  switch(props.size) {
    case 'small': return 'w-12 h-12 text-2xl'
    case 'large': return 'w-24 h-24 text-5xl'
    default: return 'w-16 h-16 text-3xl'
  }
})
</script>

<template>
  <div class="character-avatar-container" :class="sizeClasses">
    <div class="avatar-frame hd2d-style" :class="[classAvatarConfig.borderColor]">
      <!-- HD2D背景層 -->
      <div class="avatar-bg absolute inset-0 rounded-lg bg-gradient-to-br"
           :class="classAvatarConfig.bgGradient">
        <div class="hd2d-texture"></div>
      </div>

      <!-- HD2D光暈效果 -->
      <div v-if="showEffect" class="avatar-glow absolute inset-0 rounded-lg"
           :style="`box-shadow: 0 0 30px ${classAvatarConfig.glowColor}`"></div>

      <!-- 角色精靈 -->
      <div class="avatar-sprite relative z-10 flex items-center justify-center h-full"
           v-html="classAvatarConfig.sprite">
      </div>

      <!-- HD2D邊框裝飾 -->
      <div class="avatar-border absolute inset-0 rounded-lg pointer-events-none">
        <div class="corner-decoration top-left"></div>
        <div class="corner-decoration top-right"></div>
        <div class="corner-decoration bottom-left"></div>
        <div class="corner-decoration bottom-right"></div>
      </div>

      <!-- 職業標籤 -->
      <div v-if="size !== 'small'"
           class="absolute -bottom-6 left-1/2 transform -translate-x-1/2 bg-black/80 px-2 py-0.5 rounded text-xs whitespace-nowrap"
           :style="`color: ${classAvatarConfig.color}`">
        {{ classAvatarConfig.name }}
      </div>
    </div>
  </div>
</template>

<style scoped>
.character-avatar-container {
  position: relative;
  display: inline-block;
}

.avatar-frame {
  position: relative;
  width: 100%;
  height: 100%;
  border-width: 3px;
  border-radius: 0.5rem;
  overflow: hidden;
  transition: all 0.3s ease;
  transform-style: preserve-3d;
  perspective: 1000px;
}

/* HD2D紋理效果 */
.hd2d-texture {
  position: absolute;
  inset: 0;
  background-image:
    repeating-linear-gradient(45deg, transparent, transparent 2px, rgba(255,255,255,0.03) 2px, rgba(255,255,255,0.03) 4px),
    repeating-linear-gradient(-45deg, transparent, transparent 2px, rgba(0,0,0,0.1) 2px, rgba(0,0,0,0.1) 4px);
  mix-blend-mode: overlay;
}

/* 角色精靈動畫 */
.avatar-sprite {
  animation: hd2d-float 3s ease-in-out infinite;
}

@keyframes hd2d-float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-2px); }
}

/* 戰士特效 */
.warrior-sprite {
  position: relative;
  font-size: inherit;
}

.armor-shine {
  position: absolute;
  inset: -10px;
  background: linear-gradient(45deg, transparent 30%, rgba(255,255,255,0.3) 50%, transparent 70%);
  animation: shine-sweep 3s infinite;
}

.sword-glow {
  position: absolute;
  top: -5px;
  right: -5px;
  width: 20px;
  height: 20px;
  background: radial-gradient(circle, #ff6b6b, transparent);
  animation: pulse-glow 2s infinite;
}

/* 法師特效 */
.mage-sprite {
  position: relative;
  font-size: inherit;
}

.magic-aura {
  position: absolute;
  inset: -15px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(79, 195, 247, 0.3), transparent);
  animation: magic-rotate 4s linear infinite;
}

.staff-crystal {
  position: absolute;
  top: -8px;
  left: 50%;
  transform: translateX(-50%);
  width: 10px;
  height: 10px;
  background: #4fc3f7;
  border-radius: 50%;
  box-shadow: 0 0 20px #4fc3f7;
  animation: crystal-pulse 1.5s infinite;
}

/* 牧師特效 */
.priest-sprite {
  position: relative;
  font-size: inherit;
}

.holy-light {
  position: absolute;
  inset: -20px;
  background: radial-gradient(circle, rgba(255, 213, 79, 0.2), transparent);
  animation: holy-pulse 2.5s infinite;
}

.healing-aura {
  position: absolute;
  inset: 0;
  border-radius: 50%;
  border: 2px solid rgba(255, 213, 79, 0.3);
  animation: aura-expand 3s infinite;
}

/* 邊角裝飾 */
.corner-decoration {
  position: absolute;
  width: 12px;
  height: 12px;
  border: 2px solid currentColor;
  opacity: 0.7;
}

.corner-decoration.top-left {
  top: -1px;
  left: -1px;
  border-right: none;
  border-bottom: none;
}

.corner-decoration.top-right {
  top: -1px;
  right: -1px;
  border-left: none;
  border-bottom: none;
}

.corner-decoration.bottom-left {
  bottom: -1px;
  left: -1px;
  border-right: none;
  border-top: none;
}

.corner-decoration.bottom-right {
  bottom: -1px;
  right: -1px;
  border-left: none;
  border-top: none;
}

/* 動畫定義 */
@keyframes shine-sweep {
  0% { transform: translateX(-100%) translateY(-100%); }
  100% { transform: translateX(100%) translateY(100%); }
}

@keyframes pulse-glow {
  0%, 100% { opacity: 0.5; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.5); }
}

@keyframes magic-rotate {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes crystal-pulse {
  0%, 100% { transform: translateX(-50%) scale(1); }
  50% { transform: translateX(-50%) scale(1.3); }
}

@keyframes holy-pulse {
  0%, 100% { opacity: 0.3; transform: scale(1); }
  50% { opacity: 0.6; transform: scale(1.1); }
}

@keyframes aura-expand {
  0% { transform: scale(0.8); opacity: 0; }
  50% { transform: scale(1); opacity: 0.5; }
  100% { transform: scale(1.2); opacity: 0; }
}

/* Hover效果 */
.avatar-frame:hover {
  transform: translateY(-2px) scale(1.05);
  box-shadow: 0 10px 30px rgba(0,0,0,0.5);
}

.avatar-frame:hover .avatar-sprite {
  animation-duration: 1.5s;
}
</style>