---
description: 使用完整 SDD 流程開發遊戲功能（設計先行），整合所有專家 Agent 協作
---

# SDD 遊戲功能開發流程（完整版）

## 輸入參數
功能描述: $ARGUMENTS

## 完整流程概覽

```
┌─────────────────────────────────────────────────────────────────┐
│                 SDD 遊戲開發流程（完整版）                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Step 0: Constitution Check                                     │
│     └─→ 確認專案原則存在                                         │
│                                                                 │
│  Step 1: Design (game-designer) ⭐ 新增！                        │
│     └─→ 遊戲設計：機制、數值、體驗                                │
│     └─→ 輸出：designs/[feature]-design.md                       │
│                                                                 │
│  Step 2: Specify (spec-writer)                                  │
│     └─→ 根據設計撰寫規格                                         │
│     └─→ 輸出：specs/[feature]/spec.md                           │
│     └─→ [quality-reviewer 審查]                                 │
│                                                                 │
│  Step 3: Plan (tech-planner + architect)                        │
│     └─→ 技術規劃、架構設計                                       │
│     └─→ 輸出：specs/[feature]/plan.md, research.md              │
│     └─→ [quality-reviewer 審查]                                 │
│                                                                 │
│  Step 4: Tasks (task-breakdown)                                 │
│     └─→ 任務分解、依賴分析                                       │
│     └─→ 輸出：specs/[feature]/tasks.md                          │
│     └─→ [quality-reviewer 審查]                                 │
│                                                                 │
│  Step 5: Implement (implementer)                                │
│     └─→ 逐一執行任務                                            │
│     └─→ [qa-tester 撰寫測試]                                    │
│                                                                 │
│  Step 6: Refactor (refactoring-expert) [可選]                   │
│     └─→ 程式碼優化                                              │
│                                                                 │
│  Step 7: Final Review (quality-reviewer)                        │
│     └─→ 最終驗收                                                │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Agent 分工表

| 步驟 | 主要 Agent | 支援 Agent | 產出 |
|------|-----------|-----------|------|
| Design | game-designer | - | `*-design.md` |
| Specify | spec-writer | game-designer (諮詢) | `spec.md` |
| Plan | tech-planner | architect | `plan.md`, `research.md` |
| Tasks | task-breakdown | - | `tasks.md` |
| Implement | implementer | qa-tester | 程式碼 + 測試 |
| Refactor | refactoring-expert | - | 優化程式碼 |
| Review | quality-reviewer | - | 審查報告 |

## 執行步驟

### Step 0: 初始化檢查
1. 確認 `.specify/memory/constitution.md` 存在
2. 確認 `.claude/shared/game-design-doc.md` 存在（整體遊戲設計）
3. 建立功能分支：`git checkout -b feature/[feature-name]`

---

### Step 1: 遊戲設計 (Design) ⭐
使用 **game-designer** agent：

**輸入**：功能需求描述
**任務**：
1. 分析功能在遊戲中的定位
2. 設計核心玩法循環
3. 規劃數值系統
4. 設計玩家體驗
5. 考慮與其他系統的互動

**輸出**：`.claude/shared/designs/[feature]-design.md`

**範例 Prompt**：
```
使用 game-designer 設計背包系統：
- 玩家可以管理收集到的道具
- 需要與戰鬥系統、經濟系統連動
- 要有滿足感的收集體驗
```

**完成後**：通知 spec-writer 開始撰寫規格

---

### Step 2: 撰寫規格 (Specify)
使用 **spec-writer** agent：

**輸入**：遊戲設計文件
**任務**：
1. 閱讀 `[feature]-design.md`
2. 將設計機制轉為用戶故事
3. 撰寫驗收標準
4. 識別 Edge Cases

**輸出**：`.specify/specs/[feature]/spec.md`

**如果設計有疑問**：可以召喚 game-designer 諮詢
```
這個驗收標準的數值設計合理嗎？請 game-designer 確認
```

**審查點**：使用 quality-reviewer 審查規格完整性

---

### Step 3: 技術規劃 (Plan)
使用 **tech-planner** agent + **architect** agent：

**輸入**：規格文件
**任務**：
1. 選擇技術棧
2. 設計系統架構
3. 定義介面和資料模型
4. 進行技術研究

**輸出**：
- `.specify/specs/[feature]/plan.md`
- `.specify/specs/[feature]/research.md`

**架構問題**：召喚 architect 諮詢
```
這個架構設計是否符合 SOLID 原則？請 architect 審查
```

**審查點**：使用 quality-reviewer 審查技術計畫

---

### Step 4: 任務分解 (Tasks)
使用 **task-breakdown** agent：

**輸入**：技術計畫
**任務**：
1. 分解為可執行任務
2. 建立依賴關係
3. 識別可並行任務
4. 設定檢查點

**輸出**：`.specify/specs/[feature]/tasks.md`

**審查點**：使用 quality-reviewer 審查任務分解

---

### Step 5: 實作 (Implement)
使用 **implementer** agent + **qa-tester** agent：

**輸入**：任務清單
**任務**：
1. 按順序執行任務
2. 每個任務完成後執行測試
3. TDD 任務由 qa-tester 先寫測試

**執行模式**：
```
對於每個任務：
  if 任務是 [TDD]:
    qa-tester 撰寫測試
  implementer 實作程式碼
  執行測試
  確認通過
```

---

### Step 6: 重構 (Refactor) [可選]
使用 **refactoring-expert** agent：

**輸入**：已實作的程式碼
**任務**：
1. 識別 Code Smells
2. 提出重構建議
3. 執行重構

**觸發條件**：
- 程式碼複雜度過高
- 有重複程式碼
- 不符合 constitution 規範

---

### Step 7: 最終驗收 (Final Review)
使用 **quality-reviewer** agent：

**任務**：
1. 對照規格驗證功能
2. 檢查所有驗收標準
3. 確認測試覆蓋率
4. 產出驗收報告

---

## 流程控制

### 暫停與繼續
你可以在任何步驟暫停：
```
先暫停，我想調整一下設計
```

### 回退步驟
發現問題可以回到前面的步驟：
```
這個規格有問題，回到 Step 2 修改
```

### 跳過步驟
對於簡單功能，可以跳過某些步驟：
```
這個功能很簡單，跳過 Step 6 (Refactor)
```

### 召喚特定 Agent
在任何步驟都可以召喚其他 Agent 諮詢：
```
請 architect 看一下這個設計
請 game-designer 確認這個數值平衡
```

---

## 開始執行

我會依照以下順序執行：

1. **確認 Constitution** - 檢查專案原則
2. **呼叫 game-designer** - 進行遊戲設計
3. **呼叫 spec-writer** - 根據設計撰寫規格
4. **呼叫 quality-reviewer** - 審查規格
5. **呼叫 tech-planner** - 技術規劃
6. **呼叫 quality-reviewer** - 審查計畫
7. **呼叫 task-breakdown** - 分解任務
8. **呼叫 quality-reviewer** - 審查任務
9. **呼叫 implementer + qa-tester** - 實作
10. **呼叫 quality-reviewer** - 最終驗收

每個步驟完成後，我會：
- 展示產出
- 詢問是否需要調整
- 確認後進入下一步

**準備好了嗎？請描述你想開發的功能！**
