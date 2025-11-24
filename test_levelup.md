# 測試死亡後升級修復

## 修復說明
已修正玩家死亡後經驗值足夠但無法升級的問題。

## 當前狀態
✅ 後端服務運行中 (http://localhost:5000)
✅ 修復已部署並生效

## 修復內容
在 `Backend/Services/CombatSession.cs` 的死亡處理邏輯中添加了升級檢查：

```csharp
// 死亡後檢查是否能升級
var expNeeded = GetExpForNextLevel(_player.Level);
bool leveledUp = false;
while (_player.Exp >= expNeeded)
{
    _player.Exp -= expNeeded;
    _player.Level++;
    LevelUp(_player);
    leveledUp = true;
    log.Add($"🎉 恭喜升級！現在是等級 {_player.Level}！");
    expNeeded = GetExpForNextLevel(_player.Level);
}

if (leveledUp)
{
    _player.CurrentHp = _player.MaxHp;
    _player.CurrentMp = _player.MaxMp;
    log.Add("升級後生命值和魔力值已完全恢復！");
}
```

## 測試步驟
1. 登入帳號 harry2
2. 確認當前等級和經驗值
3. 進行戰鬥
4. 如果死亡，檢查是否有足夠經驗值升級
5. 驗證死亡後若經驗足夠會自動升級

## 預期結果
- 玩家死亡後若經驗值足夠將自動升級
- 升級後生命值和魔力值完全恢復
- 顯示升級訊息："🎉 恭喜升級！現在是等級 X！"