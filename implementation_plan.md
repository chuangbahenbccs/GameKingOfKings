# Implementation Plan - King of Kings (Web Version)

# Goal Description
Implement a modern web-based text RPG inspired by "King of Kings". The game will feature a responsive web interface and a robust backend engine.

## User Review Required
> [!NOTE]
> **Tech Stack Confirmed**:
> - **Frontend**: Vue 3 (Vite) + TailwindCSS
> - **Backend**: C# .NET 7/8 Web API
> - **Real-time**: SignalR (Essential for game loop/chat updates)
> - **Database**: SQLite (via Entity Framework Core)

## Code Architecture & Naming Conventions

### General Rules
- **Backend (C#)**:
    - **Classes/Models/Methods**: `PascalCase` (e.g., `PlayerCharacter`, `CalculateDamage`).
    - **Variables/Parameters**: `camelCase` (e.g., `targetId`, `damageAmount`).
    - **Interfaces**: Prefix with `I` (e.g., `IGameService`).
    - **Constants**: `PascalCase` (e.g., `MaxInventorySize`).
- **Frontend (TypeScript/Vue)**:
    - **Components**: `PascalCase` (e.g., `GameTerminal.vue`).
    - **Files**: `PascalCase` for components, `camelCase` for logic/utils.
    - **Variables/Functions**: `camelCase` (e.g., `handleCommand`, `currentUser`).
    - **Types/Interfaces**: `PascalCase` (e.g., `PlayerStats`, `ChatMessage`).

### Backend Architecture (C#)

#### 1. Models (Entity Framework Core)
Located in `Backend/Models/`
- **`User`**: Account information.
    - `Id` (Guid), `Username` (string), `PasswordHash` (string), `CreatedAt` (DateTime).
- **`PlayerCharacter`**: In-game character.
    - `Id` (Guid), `UserId` (Guid), `Name` (string), `ClassType` (Enum: Warrior, Mage, Priest), `Level` (int), `Exp` (long).
    - `CurrentHp` (int), `CurrentMp` (int).
    - `Stats` (Owned Entity): `Str`, `Int`, `Wis`, `Dex`, `Con`.
    - `CurrentRoomId` (int).
- **`WorldRoom`**: Static world data.
    - `Id` (int), `Name` (string), `Description` (string).
    - `Exits` (JSON string or related entity): Dictionary of direction (N/S/E/W) to RoomId.
- **`Item`**: Game items.
    - `Id` (int), `Name` (string), `Type` (Enum: Weapon, Armor, Consumable).

#### 2. DTOs (Data Transfer Objects)
Located in `Backend/DTOs/`
- **`LoginRequest`**: `Username`, `Password`.
- **`CharacterCreateRequest`**: `Name`, `ClassType`.
- **`GameStateDto`**: Snapshot of current player state for client updates.

#### 3. Services
Located in `Backend/Services/`
- **`IGameEngine` / `GameEngine`**: Singleton. Manages the active game loop.
    - `ProcessCommand(Guid playerId, string command)`
    - `Tick()`: Runs every 1-3 seconds (regen, combat rounds).
- **`IAuthService` / `AuthService`**: Handles JWT generation and password hashing.

#### 4. SignalR Hub
Located in `Backend/Hubs/`
- **`GameHub`**:
    - **Server Methods** (Called by Client):
        - `JoinGame(string token)`: Authenticate and map ConnectionId to PlayerId.
        - `SendCommand(string input)`: Process user input (e.g., "kill slime", "north").
    - **Client Methods** (Called by Server):
        - `ReceiveMessage(string type, string content)`: Types: "chat", "combat", "system".
        - `UpdateStats(StatsDto stats)`: Real-time HP/MP updates.

### Frontend Architecture (Vue 3)

#### 1. Components
Located in `Frontend/src/components/`
- **`GameTerminal.vue`**: Main text output area. Handles scrolling and HTML rendering.
- **`CommandInput.vue`**: Input field with history (Up/Down arrows).
- **`StatusPanel.vue`**: Displays HP/MP bars and current location.
- **`LoginScreen.vue`**: Form for auth and character selection.

#### 2. State Management (Composables/Pinia)
Located in `Frontend/src/stores/` or `composables/`
- **`useGameStore`**:
    - State: `messages[]`, `playerStats`, `isConnected`.
    - Actions: `addMessage()`, `updateStats()`.
- **`useSignalR`**:
    - Manages the HubConnection.
    - Exposes `connect()`, `send()`, and registers handlers for `ReceiveMessage`.

#### 3. Types
Located in `Frontend/src/types/`
- **`GameMessage`**: `{ id: string, type: 'info'|'combat'|'error', content: string, timestamp: Date }`
- **`PlayerStats`**: `{ hp: number, maxHp: number, mp: number, maxMp: number, name: string, level: number }`

## Proposed Changes (Phase 2 Focus)

### Backend
#### [NEW] `Backend/Models/PlayerCharacter.cs`
- Define the EF Core entity with stats.

#### [NEW] `Backend/Services/GameEngine.cs`
- Implement basic command parsing ("look", "say", "move").

#### [MODIFY] `Backend/Hubs/GameHub.cs`
- Integrate `GameEngine` to process commands instead of simple echo.

### Frontend
#### [NEW] `Frontend/src/services/signalr.ts`
- Abstract SignalR logic away from components.

#### [MODIFY] `Frontend/src/components/GameTerminal.vue`
- Use `signalr.ts` service.
- Support different message types (colors).

## Verification Plan
### Automated Tests
- **Backend**: xUnit tests for `GameEngine.ProcessCommand`.
- **Frontend**: Vitest for `GameTerminal` message rendering.
### Manual Verification
- **Connection**: Verify SignalR connection establishes on login.
- **Game Loop**: Verify commands sent from Vue reach C# and response is pushed back.
- **Persistence**: Restart server and ensure character location is saved.
