import { defineStore } from 'pinia'
import { ref } from 'vue'

// Define class types matching backend
export const ClassType = {
    Warrior: 0,
    Mage: 1,
    Priest: 2
} as const;

export type ClassType = typeof ClassType[keyof typeof ClassType];

export const SkillType = {
    Damage: 0,
    Heal: 1,
    Buff: 2,
    Debuff: 3
} as const;

export type SkillType = typeof SkillType[keyof typeof SkillType];

// Character stats interface
export interface CharacterStats {
    str: number;
    dex: number;
    int: number;
    wis: number;
    con: number;
}

// Skill interface
export interface Skill {
    id: string;
    name: string;
    description: string;
    requiredClass: ClassType;
    manaCost: number;
    cooldownSeconds: number;
    damageMultiplier: number;
    baseEffectValue: number;
    type: SkillType;
}

// Define the Player interface based on the backend model
// 根據後端模型定義 Player 介面
export interface Player {
    name: string;
    class: ClassType;
    level: number;
    exp: number;
    currentHp: number;
    maxHp: number;
    currentMp: number;
    maxMp: number;
    stats: CharacterStats;
    currentRoomId: number;
    skills?: Skill[];
}

export const usePlayerStore = defineStore('player', () => {
    // State
    const player = ref<Player | null>(null);
    const isConnected = ref(false);

    // Actions
    function setPlayer(data: Player) {
        player.value = data;
    }

    function updateHp(current: number, max: number) {
        if (player.value) {
            player.value.currentHp = current;
            player.value.maxHp = max;
        }
    }

    return {
        player,
        isConnected,
        setPlayer,
        updateHp
    }
})
