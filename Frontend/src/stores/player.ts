import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

// Define the Player interface based on the backend model
// 根據後端模型定義 Player 介面
export interface Player {
    name: string;
    currentHp: number;
    maxHp: number;
    currentMp: number;
    maxMp: number;
    level: number;
    exp: number;
    className?: string;
}

// Combat state interface
// 戰鬥狀態介面
export interface CombatState {
    inCombat: boolean;
    monsterName: string;
    monsterHp: number;
    monsterMaxHp: number;
    monsterEmoji: string;
}

export const usePlayerStore = defineStore('player', () => {
    // State
    const player = ref<Player | null>(null);
    const isConnected = ref(false);
    const combat = ref<CombatState>({
        inCombat: false,
        monsterName: '',
        monsterHp: 0,
        monsterMaxHp: 0,
        monsterEmoji: '🐺'
    });

    // Computed
    const hpPercent = computed(() => {
        if (!player.value) return 0;
        return (player.value.currentHp / player.value.maxHp) * 100;
    });

    const mpPercent = computed(() => {
        if (!player.value) return 0;
        return (player.value.currentMp / player.value.maxMp) * 100;
    });

    const expPercent = computed(() => {
        if (!player.value) return 0;
        const expRequired = player.value.level * 100;
        return (player.value.exp / expRequired) * 100;
    });

    const monsterHpPercent = computed(() => {
        if (!combat.value.inCombat || combat.value.monsterMaxHp === 0) return 0;
        return (combat.value.monsterHp / combat.value.monsterMaxHp) * 100;
    });

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

    function updateMp(current: number, max: number) {
        if (player.value) {
            player.value.currentMp = current;
            player.value.maxMp = max;
        }
    }

    function updateExp(exp: number) {
        if (player.value) {
            player.value.exp = exp;
        }
    }

    function levelUp(newLevel: number) {
        if (player.value) {
            player.value.level = newLevel;
        }
    }

    function startCombat(monsterName: string, monsterHp: number, monsterMaxHp: number) {
        combat.value = {
            inCombat: true,
            monsterName,
            monsterHp,
            monsterMaxHp,
            monsterEmoji: getMonsterEmoji(monsterName)
        };
    }

    function updateMonsterHp(hp: number, maxHp?: number) {
        combat.value.monsterHp = hp;
        if (maxHp !== undefined) {
            combat.value.monsterMaxHp = maxHp;
        }
    }

    function endCombat() {
        combat.value = {
            inCombat: false,
            monsterName: '',
            monsterHp: 0,
            monsterMaxHp: 0,
            monsterEmoji: '🐺'
        };
    }

    function setConnected(connected: boolean) {
        isConnected.value = connected;
    }

    // Helper function to get monster emoji
    function getMonsterEmoji(name: string): string {
        const lowerName = name.toLowerCase();
        if (lowerName.includes('slime')) return '🟢';
        if (lowerName.includes('rat')) return '🐀';
        if (lowerName.includes('wolf')) return '🐺';
        if (lowerName.includes('goblin')) return '👺';
        if (lowerName.includes('skeleton')) return '💀';
        if (lowerName.includes('zombie')) return '🧟';
        if (lowerName.includes('king')) return '👑';
        return '👾';
    }

    return {
        // State
        player,
        isConnected,
        combat,
        // Computed
        hpPercent,
        mpPercent,
        expPercent,
        monsterHpPercent,
        // Actions
        setPlayer,
        updateHp,
        updateMp,
        updateExp,
        levelUp,
        startCombat,
        updateMonsterHp,
        endCombat,
        setConnected
    }
})
