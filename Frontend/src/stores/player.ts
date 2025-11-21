import { defineStore } from 'pinia'
import { ref } from 'vue'

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
