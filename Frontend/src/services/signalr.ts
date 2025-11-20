import * as signalR from '@microsoft/signalr'
import { ref } from 'vue'

class SignalRService {
    private connection: signalR.HubConnection
    public messages = ref<string[]>([])
    public isConnected = ref(false)

    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/gameHub")
            .withAutomaticReconnect()
            .build()

        this.connection.on("ReceiveMessage", (user: string, message: string) => {
            const formattedMsg = `<span class="font-bold text-yellow-400">${user}</span>: ${message}`
            this.messages.value.push(formattedMsg)
        })
    }

    public async start() {
        try {
            await this.connection.start()
            this.isConnected.value = true
            this.messages.value.push("Connected to Game Server.")
        } catch (err) {
            console.error(err)
            this.messages.value.push("Error connecting to server.")
        }
    }

    public async joinGame(username: string) {
        if (!this.isConnected.value) return
        await this.connection.invoke("JoinGame", username)
    }

    public async sendCommand(command: string) {
        if (!this.isConnected.value) return
        await this.connection.invoke("SendCommand", command)
    }
}

export const signalRService = new SignalRService()
