<script setup lang="ts">
import { computed } from 'vue'
import { usePlayerStore } from '../stores/player'
import CharacterAvatar from './CharacterAvatar.vue'

const playerStore = usePlayerStore()

interface Props {
  monster?: any
  isFighting?: boolean
}

const props = defineProps<Props>()

// 獲取玩家角色精靈樣式
const playerSpriteStyle = computed(() => {
  if (!playerStore.player) return {}

  const classType = playerStore.player.class
  switch(classType) {
    case 0: // Warrior
      return {
        emoji: '🗡️',
        color: '#ff6b6b',
        animation: 'warrior-idle'
      }
    case 1: // Mage
      return {
        emoji: '🧙',
        color: '#4fc3f7',
        animation: 'mage-idle'
      }
    case 2: // Priest
      return {
        emoji: '⚕️',
        color: '#ffd54f',
        animation: 'priest-idle'
      }
    default:
      return {
        emoji: '👤',
        color: '#9e9e9e',
        animation: 'idle'
      }
  }
})

// 場景背景根據位置變化
const sceneBackground = computed(() => {
  const roomId = playerStore.player?.currentRoomId || 1
  switch(roomId) {
    case 1: // 村莊廣場
      return 'village-square'
    case 2: // 訓練場
      return 'training-ground'
    case 3: // 村長家
      return 'village-house'
    default:
      return 'village-square'
  }
})
</script>

<template>
  <div class="hd2d-battle-scene" :class="`scene-${sceneBackground}`">
    <!-- HD2D多層背景 -->
    <div class="scene-layers">
      <!-- 遠景層 -->
      <div class="layer layer-far">
        <div class="parallax-bg far-bg"></div>
      </div>

      <!-- 中景層 -->
      <div class="layer layer-mid">
        <div class="parallax-bg mid-bg"></div>
        <!-- 環境元素 -->
        <div class="environment-elements">
          <div class="element torch torch-1">🔥</div>
          <div class="element torch torch-2">🔥</div>
          <div class="element decoration">🏺</div>
        </div>
      </div>

      <!-- 近景層（戰鬥平台） -->
      <div class="layer layer-near">
        <div class="battle-platform">
          <!-- 地板紋理 -->
          <div class="platform-texture"></div>

          <!-- 玩家角色區域 -->
          <div class="player-area" v-if="isFighting">
            <div class="character-container player-character">
              <!-- HD2D玩家精靈 -->
              <div class="character-sprite" :class="playerSpriteStyle.animation">
                <CharacterAvatar
                  :class-type="playerStore.player?.class || 0"
                  size="large"
                  :show-effect="true"
                />
                <!-- 角色狀態條 -->
                <div class="character-status">
                  <div class="hp-bar mini">
                    <div class="hp-fill"
                         :style="{width: `${(playerStore.player?.currentHp / playerStore.player?.maxHp * 100) || 100}%`}">
                    </div>
                  </div>
                  <div class="mp-bar mini">
                    <div class="mp-fill"
                         :style="{width: `${(playerStore.player?.currentMp / playerStore.player?.maxMp * 100) || 100}%`}">
                    </div>
                  </div>
                </div>
              </div>

              <!-- 角色陰影 -->
              <div class="character-shadow"></div>
            </div>
          </div>

          <!-- 怪物區域 -->
          <div class="monster-area" v-if="monster">
            <div class="character-container monster-character">
              <!-- 怪物精靈 -->
              <div class="monster-sprite hd2d-sprite">
                <span class="monster-emoji">{{ monster.image }}</span>
              </div>

              <!-- 怪物陰影 -->
              <div class="monster-shadow"></div>

              <!-- HP條 -->
              <div class="monster-hp-container">
                <div class="hp-bar">
                  <div class="hp-fill enemy"
                       :style="{width: `${(monster.currentHp / monster.maxHp * 100)}%`}">
                  </div>
                </div>
                <div class="monster-name">{{ monster.name }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 前景裝飾層 -->
      <div class="layer layer-foreground">
        <div class="foreground-elements">
          <!-- HD2D風格的粒子效果 -->
          <div class="particle-system">
            <span v-for="i in 10" :key="i" class="particle" :style="`--i: ${i}`"></span>
          </div>
        </div>
      </div>
    </div>

    <!-- HD2D光影效果 -->
    <div class="lighting-effects">
      <div class="ambient-light"></div>
      <div class="volumetric-light"></div>
    </div>
  </div>
</template>

<style scoped>
.hd2d-battle-scene {
  position: relative;
  width: 100%;
  height: 100%;
  overflow: hidden;
  background: linear-gradient(to bottom, #2a1a3e, #1a0e2e);
}

/* 場景層級系統 */
.scene-layers {
  position: relative;
  width: 100%;
  height: 100%;
  perspective: 1200px;
  transform-style: preserve-3d;
}

.layer {
  position: absolute;
  width: 100%;
  height: 100%;
  transform-origin: center bottom;
}

/* 遠景層 */
.layer-far {
  transform: translateZ(-300px) scale(1.3);
}

.far-bg {
  width: 100%;
  height: 60%;
  background: linear-gradient(to bottom,
    rgba(138, 43, 226, 0.2),
    rgba(75, 0, 130, 0.3));
  filter: blur(2px);
}

/* 中景層 */
.layer-mid {
  transform: translateZ(-150px) scale(1.15);
}

.mid-bg {
  position: absolute;
  bottom: 0;
  width: 100%;
  height: 40%;
  background: linear-gradient(to top,
    rgba(52, 31, 23, 0.8),
    transparent);
}

/* 環境元素 */
.environment-elements {
  position: absolute;
  width: 100%;
  height: 100%;
}

.element {
  position: absolute;
  font-size: 2rem;
  filter: drop-shadow(0 10px 20px rgba(0,0,0,0.5));
}

.torch {
  animation: torch-flicker 2s infinite;
}

.torch-1 {
  left: 10%;
  top: 30%;
}

.torch-2 {
  right: 10%;
  top: 30%;
}

.decoration {
  bottom: 20%;
  left: 5%;
}

@keyframes torch-flicker {
  0%, 100% { transform: scale(1); filter: brightness(1); }
  50% { transform: scale(1.1); filter: brightness(1.3); }
}

/* 近景層（戰鬥平台） */
.layer-near {
  transform: translateZ(0) scale(1);
}

.battle-platform {
  position: absolute;
  bottom: 0;
  width: 100%;
  height: 50%;
  transform: rotateX(60deg) translateZ(-50px);
  transform-style: preserve-3d;
}

/* 平台紋理 */
.platform-texture {
  position: absolute;
  width: 100%;
  height: 100%;
  background:
    repeating-linear-gradient(90deg,
      #3e2723 0px,
      #4e342e 20px,
      #3e2723 40px),
    repeating-linear-gradient(0deg,
      rgba(0,0,0,0.2) 0px,
      transparent 10px,
      rgba(0,0,0,0.2) 20px);
  border-top: 3px solid #5d4037;
  box-shadow:
    0 -5px 20px rgba(0,0,0,0.5),
    inset 0 5px 20px rgba(0,0,0,0.3);
}

/* 角色容器 */
.character-container {
  position: absolute;
  transform-style: preserve-3d;
  transform: translateZ(50px);
}

/* 玩家區域 */
.player-area {
  position: absolute;
  left: 20%;
  bottom: 20%;
  transform: translateZ(100px);
}

.player-character {
  animation: character-idle 3s infinite ease-in-out;
}

/* 怪物區域 */
.monster-area {
  position: absolute;
  right: 25%;
  bottom: 25%;
  transform: translateZ(100px);
}

.monster-character {
  animation: monster-idle 4s infinite ease-in-out;
}

/* 角色精靈 */
.character-sprite {
  position: relative;
  transition: all 0.3s ease;
}

.monster-sprite {
  position: relative;
  display: inline-block;
}

.monster-emoji {
  font-size: 5rem;
  display: block;
  filter:
    drop-shadow(0 0 10px rgba(255, 100, 100, 0.5))
    drop-shadow(0 10px 20px rgba(0, 0, 0, 0.7));
  animation: monster-float 3s infinite ease-in-out;
}

/* 角色狀態條 */
.character-status {
  position: absolute;
  bottom: -30px;
  left: 50%;
  transform: translateX(-50%);
  width: 80px;
}

.hp-bar, .mp-bar {
  height: 4px;
  background: rgba(0,0,0,0.5);
  border: 1px solid #333;
  margin: 2px 0;
  border-radius: 2px;
  overflow: hidden;
}

.hp-fill {
  height: 100%;
  background: linear-gradient(to right, #ff4444, #ff6666);
  transition: width 0.3s ease;
}

.mp-fill {
  height: 100%;
  background: linear-gradient(to right, #4444ff, #6666ff);
  transition: width 0.3s ease;
}

/* 怪物HP條 */
.monster-hp-container {
  position: absolute;
  top: -40px;
  left: 50%;
  transform: translateX(-50%);
  width: 100px;
  text-align: center;
}

.monster-name {
  color: #ff6666;
  font-size: 0.875rem;
  font-weight: bold;
  text-shadow: 2px 2px 0 #000;
  margin-top: 4px;
}

.hp-fill.enemy {
  background: linear-gradient(to right, #ff0000, #ff4444);
}

/* 陰影效果 */
.character-shadow, .monster-shadow {
  position: absolute;
  bottom: -10px;
  left: 50%;
  width: 60px;
  height: 20px;
  background: radial-gradient(ellipse, rgba(0,0,0,0.7), transparent);
  transform: translateX(-50%) rotateX(90deg);
  animation: shadow-pulse 3s infinite ease-in-out;
}

.monster-shadow {
  width: 80px;
  height: 30px;
}

/* 粒子系統 */
.particle-system {
  position: absolute;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

.particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  animation: particle-float calc(10s + var(--i) * 1s) infinite linear;
  left: calc(var(--i) * 10%);
}

@keyframes particle-float {
  0% {
    transform: translateY(100vh) translateX(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100vh) translateX(50px);
    opacity: 0;
  }
}

/* 光影效果 */
.lighting-effects {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

.ambient-light {
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle at 50% 30%,
    rgba(255, 220, 150, 0.1),
    transparent 60%);
  mix-blend-mode: screen;
}

.volumetric-light {
  position: absolute;
  top: 0;
  right: 20%;
  width: 200px;
  height: 100%;
  background: linear-gradient(180deg,
    rgba(255, 255, 200, 0.2),
    transparent);
  transform: skewX(-20deg);
  filter: blur(40px);
  mix-blend-mode: screen;
  animation: light-sway 8s infinite ease-in-out;
}

@keyframes light-sway {
  0%, 100% { transform: skewX(-20deg) translateX(0); }
  50% { transform: skewX(-25deg) translateX(20px); }
}

/* 角色動畫 */
@keyframes character-idle {
  0%, 100% { transform: translateY(0) translateZ(100px); }
  50% { transform: translateY(-5px) translateZ(100px); }
}

@keyframes monster-idle {
  0%, 100% { transform: translateY(0) translateZ(100px); }
  50% { transform: translateY(-8px) translateZ(100px); }
}

@keyframes monster-float {
  0%, 100% { transform: translateY(0) scale(1); }
  50% { transform: translateY(-10px) scale(1.05); }
}

@keyframes shadow-pulse {
  0%, 100% { transform: translateX(-50%) rotateX(90deg) scale(1); }
  50% { transform: translateX(-50%) rotateX(90deg) scale(0.8); }
}

/* 場景變化 */
.scene-village-square {
  background: linear-gradient(to bottom, #4a5568, #2d3748);
}

.scene-training-ground {
  background: linear-gradient(to bottom, #7c3f00, #5c2e00);
}

.scene-village-house {
  background: linear-gradient(to bottom, #3e2723, #2e1a17);
}
</style>