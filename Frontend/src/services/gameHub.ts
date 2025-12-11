import * as signalR from "@microsoft/signalr";

// The URL should match the backend Hub URL
// URL 應與後端 Hub URL 相符
const HUB_URL = "http://localhost:5000/gameHub";

export interface StatsUpdate {
    currentHp: number;
    maxHp: number;
    monsterHp?: number;
    monsterMaxHp?: number;
}

class GameHubService {
    private connection: signalR.HubConnection;
    private messageCallbacks: ((user: string, message: string) => void)[] = [];
    private statsCallbacks: ((stats: StatsUpdate) => void)[] = [];

    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect()
            .build();
    }

    // Start the connection
    // 啟動連線
    public async start() {
        try {
            await this.connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            setTimeout(() => this.start(), 5000);
        }
    }

    // Join the game
    // 加入遊戲
    public async joinGame(username: string) {
        try {
            await this.connection.invoke("JoinGame", username);
        } catch (err) {
            console.error("JoinGame Error: ", err);
        }
    }

    // Send a command
    // 發送指令
    public async sendCommand(command: string) {
        try {
            await this.connection.invoke("SendCommand", command);
        } catch (err) {
            console.error("SendCommand Error: ", err);
        }
    }

    // Listen for messages
    // 監聽訊息
    public onReceiveMessage(callback: (user: string, message: string) => void) {
        this.messageCallbacks.push(callback);
        this.connection.on("ReceiveMessage", callback);
    }

    // Listen for stats updates
    // 監聽狀態更新
    public onUpdateStats(callback: (stats: StatsUpdate) => void) {
        this.statsCallbacks.push(callback);
        this.connection.on("UpdateStats", callback);
    }

    // Send a message (chat)
    // 發送訊息（聊天）
    public async sendMessage(user: string, message: string) {
        await this.connection.invoke("SendMessage", user, message);
    }

    // Check if connected
    // 檢查是否已連線
    public isConnected(): boolean {
        return this.connection.state === signalR.HubConnectionState.Connected;
    }

    // Get connection state
    // 取得連線狀態
    public getState(): signalR.HubConnectionState {
        return this.connection.state;
    }

    // Stop connection
    // 停止連線
    public async stop() {
        await this.connection.stop();
    }
}

export const gameHub = new GameHubService();
