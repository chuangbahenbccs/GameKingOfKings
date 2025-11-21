# Game Design Document - King of Kings (Web MUD)

## 1. Core Mechanics (核心機制)

### Stats (屬性)
- **STR (力量)**: Increases Physical Attack (Warrior primary).
- **INT (智力)**: Increases Magic Attack & MP Max (Mage primary).
- **WIS (智慧)**: Increases Healing Power & MP Regen (Priest primary).
- **DEX (敏捷)**: Increases Hit Rate, Evasion, and Critical Chance.
- **CON (體質)**: Increases HP Max and HP Regen.

### Combat System (戰鬥系統)
- **Type**: Real-time Tick-based (or Turn-based with cooldowns).
- **Flow**:
    1. Player initiates attack (`kill <target>`).
    2. Auto-attack loop starts (every 2-3 seconds).
    3. Skills can be cast manually (`cast fireball`) between auto-attacks.
- **Formulas (Draft)**:
    - `Dmg = (Atk - Def) * Random(0.9, 1.1)`
    - `Hit Chance = (Attacker_Dex * 2) / (Attacker_Dex + Target_Dex)`

### Resting (休息系統)
- Command: `rest`
- Effect: Doubles HP/MP regeneration tick rate. Cannot move or fight while resting.

---

## 2. Classes (職業設定)

### Warrior (戰士)
- **Role**: Tank / Physical DPS
- **Starting Stats**: High STR, High CON.
- **Skills**:
    - `Bash` (重擊): High damage single target.
    - `Taunt` (嘲諷): Force enemy to attack self.
    - `Iron Skin` (鐵布衫): Temp DEF boost.

### Mage (法師)
- **Role**: Magic DPS / AoE
- **Starting Stats**: High INT, Low CON.
- **Skills**:
    - `Fireball` (火球術): High damage single target.
    - `Ice Storm` (暴風雪): AoE damage + Slow.
    - `Mana Shield` (魔力護盾): Absorb damage using MP.

### Priest (牧師)
- **Role**: Healer / Buffer
- **Starting Stats**: High WIS, Balanced CON.
- **Skills**:
    - `Heal` (治療術): Restore HP.
    - `Bless` (祝福): Buff STR/DEX for target.
    - `Smite` (懲戒): Holy damage (scales with WIS).

---

## 3. World Map & Progression (地圖與掉落)

### Zone 1: Newbie Village (新手村)
- **Theme**: Peaceful, green fields.
- **Rooms**: Village Square, Training Dummy Area, Village Elder's House.
- **Monsters**:
    - `Slime` (史萊姆): Drops `Slime Gel` (Quest item).
    - `Rat` (大老鼠): Drops `Rat Tail`.
- **Boss**: `King Slime` (史萊姆王) - Drops `Novice Ring` (+1 All Stats).

### Zone 2: Whispering Forest (低語森林) - Lv 5-10
- **Theme**: Darker, foggy forest.
- **Monsters**:
    - `Wolf` (野狼): Drops `Wolf Pelt`, `Wolf Tooth`.
    - `Goblin` (哥布林): Drops `Rusty Dagger`, `Leather Armor`.
- **Boss**: `Goblin Chief` (哥布林酋長) - Drops `Chief's Axe` (Warrior Weapon).

### Zone 3: Abandoned Mine (廢棄礦坑) - Lv 10-15
- **Theme**: Dungeon, dark, narrow.
- **Monsters**:
    - `Skeleton` (骷髏): Drops `Bone Chips`.
    - `Zombie` (殭屍): Drops `Rotten Flesh`.
- **Boss**: `Necromancer` (死靈法師) - Drops `Mage Robe` (Mage Armor).

---

## 4. Itemization (裝備系統)
2. **Status Panel (Left/Top)**:
    - HP Bar (Red), MP Bar (Blue), EXP Bar (Yellow).
    - Current Location Name.
3. **Command Input (Bottom)**:
    - Text input with history (Up/Down arrow).
    - Quick Action Buttons (optional for mobile).

### Special Effects
- **Damage Numbers**: Floating text animation on combat log.
- **Screen Shake**: On critical hits or taking heavy damage.
- **Typewriter Effect**: For NPC dialogue.
