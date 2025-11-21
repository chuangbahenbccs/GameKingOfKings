import * as signalR from "@microsoft/signalr";



class GameHubService {
    private connection: signalR.HubConnection | null = null;

    // Start the connection
    public async connect(url: string, token: string) {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(url, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        try {
            await this.connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            throw err;
        }
    }

    public async joinGame(username: string) {
        if (!this.connection) return;
        await this.connection.invoke("JoinGame", username);
    }

    // Listen for messages
    public onReceiveMessage(callback: (user: string, message: string) => void) {
        if (!this.connection) return;
        this.connection.on("ReceiveMessage", callback);
    }

    // Send a message
    public async sendMessage(user: string, message: string) {
        if (!this.connection) return;
        await this.connection.invoke("SendMessage", user, message);
    }

    public async sendCommand(command: string) {
        if (!this.connection) return;
        await this.connection.invoke("SendCommand", command);
    }

    public onCombatUpdate(callback: (update: any) => void) {
        if (!this.connection) return;
        this.connection.on("CombatUpdate", callback);
    }

    public onPlayerData(callback: (playerData: any) => void) {
        if (!this.connection) return;
        this.connection.on("PlayerData", callback);
    }

    public onNeedCharacterCreation(callback: (username: string) => void) {
        if (!this.connection) return;
        this.connection.on("NeedCharacterCreation", callback);
    }

    public onCharacterCreated(callback: () => void) {
        if (!this.connection) return;
        this.connection.on("CharacterCreated", callback);
    }

    public async createCharacter(username: string, classType: number) {
        if (!this.connection) return;
        await this.connection.invoke("CreateCharacter", username, classType);
    }

    public stop() {
        this.connection?.stop();
        this.connection = null;
    }
}

export const gameHub = new GameHubService();
