import * as signalR from "@microsoft/signalr";

// The URL should match the backend Hub URL
// URL 應與後端 Hub URL 相符
const HUB_URL = "http://localhost:5000/gameHub";

class GameHubService {
    private connection: signalR.HubConnection;

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

    // Listen for messages
    // 監聽訊息
    public onReceiveMessage(callback: (user: string, message: string) => void) {
        this.connection.on("ReceiveMessage", callback);
    }

    // Send a message
    // 發送訊息
    public async sendMessage(user: string, message: string) {
        await this.connection.invoke("SendMessage", user, message);
    }
}

export const gameHub = new GameHubService();
