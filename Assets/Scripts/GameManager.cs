using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UpgradeType { Storage, Fuel, Speed, Bonus }

public class GameManager : MonoBehaviour
{
    //外部からアクセス可能かつ編集不可にしている
    public static GameManager Instance { get; private set; }
    //gold増減イベント
    public event Action OnGoldChanged;

    public event Action OnShipNameChanged;

    public event Action OnRankChanged;


    [SerializeField] private int storageLevel = 0;
    [SerializeField] private int fuelLevel = 0;
    [SerializeField] private int speedLevel = 0;
    [SerializeField] private int bonusLevel = 0;

    [SerializeField] private List<ShipStageData> allStages;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private float remainingFuelLiters = 180f;
    [SerializeField] private long gold = 0;
    [SerializeField] Dictionary<string, long> itemCounts = new Dictionary<string, long>();
    [SerializeField] Dictionary<string, long> keepCounts = new Dictionary<string, long>();
    [SerializeField] private string shipName = "ゴーゴー";
    [SerializeField] private bool notificationEnabled = true;
    [SerializeField] private bool isSailing = false;
    [SerializeField] private List<ItemData> allItems;


    public int StorageLevel => storageLevel;
    public int FuelLevel => fuelLevel;
    public int SpeedLevel => speedLevel;
    public int BonusLevel => bonusLevel;

    public int CurrentStage => currentStage;
    public int TotalStages => allStages.Count;
    public long Gold => gold;
    public string ShipName => shipName;
    public bool NotificationEnabled => notificationEnabled;
    public int Rank => storageLevel + fuelLevel + speedLevel + bonusLevel;

    private Dictionary<string, long> purchasedCounts = new Dictionary<string, long>();
    private Dictionary<string, long> purchaseWindowStarts = new Dictionary<string, long>();

    private const int DAILY_PURCHASE_LIMIT = 30;
    private const int PURCHASE_RESET_SECONDS = 86400;
    private const double PURCHASE_PRICE_RATE = 1.5;

    public float RemainingFuelLiters
    {
        get => remainingFuelLiters;
        private set
        {
            remainingFuelLiters = value;
            OnFuelChanged?.Invoke();
        }
    }

    public bool IsSailing => isSailing;


    public event Action OnFuelChanged;
    public event Action OnSailingChanged;

    public void Awake()
    {
        //既に存在している場合、自身を破壊
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (SaveSystem.HasSave())
        {
            SaveData data = SaveSystem.Load();
            LoadFromSaveData(data);
        }
        //
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(AutoSaveLoop());
    }

    public ShipStageData GetCurrentStageData()
    {
        //リストから1つずつ要素を取り出し、StageカラムがcurrentStageと一致するものだけを取り出す(LINQ機能)
        return allStages.Find(s => s.Stage == currentStage);
    }



    public void ConsumeFuel(float hours)
    {
        RemainingFuelLiters = Mathf.Max(0f, RemainingFuelLiters - GetCurrentFuelConsumptionRate() * hours);
    }

    //燃費計算
    public float GetCurrentFuelConsumptionRate()
    {   //最大で18時間持つように設計
        float duration = 4f + 14f * fuelLevel / 30f;
        return 180f / duration;
    }

    //Upgradeでの時間比較用
    public float GetFuelDurationHoursAtLevel(int level)
    {
        int clamped = Mathf.Clamp(level, 0, 30);
        return 4f + 14f * clamped / 30f;
    }

    public (string itemId, long amount) GetUpgradeMaterials(UpgradeType type)
    {
        int currentLevel;
        string materialId;
        int baseAmount;

        switch (type)
        {
            case UpgradeType.Storage:
                currentLevel = storageLevel;
                materialId = "wood";
                baseAmount = 3;
                break;
            case UpgradeType.Fuel:
                currentLevel = fuelLevel;
                materialId = "dirty_oil";
                baseAmount = 2;
                break;
            case UpgradeType.Speed:
                currentLevel = speedLevel;
                materialId = "scrap";
                baseAmount = 3;
                break;
            case UpgradeType.Bonus:
                currentLevel = bonusLevel;
                materialId = "cloth";
                baseAmount = 3;
                break;
            default:
                return ("", 0);
        }

        if (currentLevel < 10)
        {
            return ("", 0);
        }

        //Lv11以上は素材計算
        long amount = (long)Mathf.CeilToInt(baseAmount * Mathf.Pow(1.076f, currentLevel - 10));
        return (materialId, amount);
    }

    public ItemData GetItemData(string itemId)
    {
        foreach (var item in allItems)
        {
            if (item.ItemId == itemId)
            {
                return item;
            }
        }
        return null;
    }

    //データ増加
    public (bool success, long affordableLiters) TryRefuel(long requestedLiters)
    {
        if (requestedLiters <= 0)
        {
            return (false, 0);
        }
        long cost = GetRefuelCost(requestedLiters);

        if (!SpendGold(cost))
        {
            return (false, GetMaxAffordableLiters());
        }
        RemainingFuelLiters = Mathf.Clamp(RemainingFuelLiters + requestedLiters, 0, 180);

        return (true, requestedLiters);
    }

    public void AddGold(long amount)
    {
        gold += amount;
        //?:nullでなければ変更を知らせる
        OnGoldChanged?.Invoke();
    }

    public bool SpendGold(long amount)
    {
        if (Gold >= amount)
        {
            AddGold(-amount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddItem(ItemData item, long amount)
    {
        if (!itemCounts.ContainsKey(item.ItemId))
        {
            itemCounts[item.ItemId] = 0;
        }
        if (!keepCounts.ContainsKey(item.ItemId))
        {
            keepCounts[item.ItemId] = item.DefaultKeepCount;
        }
        itemCounts[item.ItemId] += amount;
    }

    //データ取得関連
    public float GetCurrentStorageMinutes()
    {
        ShipStageData currentData = GetCurrentStageData();

        //Stage内レべルを取得(相対レベル) 1レベルごとに20分ずつ増加
        int relativeLevel = storageLevel - (currentStage - 1) * 10;

        if (relativeLevel >= 10)
        {
            ShipStageData nextData = allStages.Find(s => s.Stage == currentStage + 1);
            if (nextData == null)
            {
                //Stage上限用
                return currentData.Storage * 60f + 9 * 20f;
            }
            return nextData.Storage * 60f;
        }
        else
        {
            return currentData.Storage * 60f + relativeLevel * 20f;
        }
    }

    //public float GetCurrentFuelMinutes()
    //{
    //    ShipStageData currentData = GetCurrentStageData();

    //    //Stage内レベルを取得 1レベルごとに20分ずつ増加
    //    int relativeLevel = fuelLevel - (currentStage - 1) * 10;

    //    if (relativeLevel >= 10)
    //    {
    //        ShipStageData nextData = allStages.Find(s => s.Stage == currentStage + 1);
    //        if (nextData == null)
    //        {
    //            //Stage上限用
    //            return currentData.FuelDuration * 60f + 9 * 20f;
    //        }
    //        return nextData.FuelDuration * 60f;
    //    }
    //    else
    //    {
    //        return currentData.FuelDuration * 60f + relativeLevel * 20f;
    //    }
    //}

    public float GetIntervalReductionPercent()
    {
        //1Lvあたり2.22%短縮、時間比較はGetIntervalSecondsAtLevel関数にて行う
        int clampedSpeedLevel = Mathf.Clamp(speedLevel, 0, 30);
        return clampedSpeedLevel / 30f * 66.7f;
    }

    //speedLevel間での秒数比較用
    public float GetIntervalSecondsAtLevel(int level)
    {
        float percent = Mathf.Clamp(level, 0, 30) / 30f * 66.7f;
        return 60f * (1f - percent / 100);
    }

    public (int min, int max) GetBonusGoldRange()
    {
        int clampedLevel = Mathf.Clamp(bonusLevel, 0, 30);
        float ratio = Mathf.Sqrt((float)clampedLevel / 30f);
        //最小値10-40　序盤に強化恩恵を感じられるような式に
        int min = Mathf.RoundToInt(10f + 30f * ratio);
        //最大値20-100
        int max = Mathf.RoundToInt(20f + 80f * ratio);
        return (min, max);
    }

    public float GetNormalBonusPercent()
    {
        int relativeLevel = Mathf.Clamp(bonusLevel - 10, 0, 10);
        return relativeLevel * 3f;
    }

    public float GetBigBonusPercent()
    {
        int relativeLevel = Mathf.Clamp(bonusLevel - 20, 0, 10);
        return relativeLevel * 1f;
    }

    public long GetRefuelCost(long liters)
    {
        //L単価100G
        return liters * 100;
    }

    public long GetMaxAffordableLiters()
    {
        long tankSpace = (long)(180 - RemainingFuelLiters);
        long affordableByGold = Gold / 100;
        return Math.Min(tankSpace, affordableByGold);
    }

    public bool HasItem(string itemId)
    {
        if (!itemCounts.ContainsKey(itemId))
        {
            return false;
        }
        return itemCounts[itemId] > 0;
    }

    public long GetItemCount(string itemId)
    {
        if (!itemCounts.ContainsKey(itemId))
        {
            return 0;
        }
        return itemCounts[itemId];
    }

    public long GetKeepCount(string itemId)
    {
        if (!keepCounts.ContainsKey(itemId))
        {
            return 0;
        }
        return keepCounts[itemId];
    }

    public List<ItemData> GetAvailableItems()
    {
        //取得したものを保存しておく
        HashSet<string> addedIds = new HashSet<string>();
        //戻り値用　現ステージで入手可能なアイテム
        List<ItemData> result = new List<ItemData>();
        foreach (var stage in allStages)
        {
            if (stage.Stage > currentStage)
            {
                continue;
            }
            foreach (var drop in stage.DropRates)
            {
                //まだ追加されていなければ
                //result操作よりも圧倒的に高効率
                if (addedIds.Add(drop.item.ItemId))
                {
                    result.Add(drop.item);
                }
            }
        }
        return result;
    }

    public float GetStorageMinutesAtLevel(int level)
    {
        level = Mathf.Clamp(level, 0, 30);
        ShipStageData currentData = GetCurrentStageData();
        int relativeLevel = level - (currentStage - 1) * 10;

        if (relativeLevel >= 10)
        {
            ShipStageData nextData = allStages.Find(s => s.Stage == currentStage + 1);
            if (nextData == null)
            {
                return currentData.Storage * 60f + 9 * 20f;
            }
            return nextData.Storage * 60f;
        }
        else
        {
            return currentData.Storage * 60f + relativeLevel * 20f;
        }
    }

    public (int min, int max) GetBonusGoldRangeAtLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 0, 30);
        float ratio = Mathf.Sqrt((float)clampedLevel / 30f);
        int min = Mathf.RoundToInt(10f + 30f * ratio);
        int max = Mathf.RoundToInt(20f + 80f * ratio);
        return (min, max);
    }

    public float GetNormalBonusPercentAtLevel(int level)
    {
        int relativeLevel = Mathf.Clamp(level - 10, 0, 10);
        return relativeLevel * 3f;
    }

    public float GetBigBonusPercentAtLevel(int level)
    {
        int relativeLevel = Mathf.Clamp(level - 20, 0, 10);
        return relativeLevel * 1f;
    }

    public long GetUpgradeCost(UpgradeType type)
    {
        int currentLevel;
        float ratio;

        switch (type)
        {
            case UpgradeType.Storage:
                currentLevel = storageLevel;
                ratio = 0.8f;
                break;
            case UpgradeType.Fuel:
                currentLevel = fuelLevel;
                ratio = 1.0f;
                break;
            case UpgradeType.Speed:
                currentLevel = speedLevel;
                ratio = 1.2f;
                break;
            case UpgradeType.Bonus:
                currentLevel = bonusLevel;
                ratio = 1.3f;
                break;
            default:
                return 0;
        }

        float cost = 500f * ratio * Mathf.Pow(1.25f, currentLevel);
        return (long)Mathf.Round(cost);
    }
    //Upgrade処理
    public bool CanUpgrade(UpgradeType type)
    {
        int currentLevel;

        switch (type)
        {
            case UpgradeType.Storage:
                currentLevel = storageLevel;
                break;
            case UpgradeType.Fuel:
                currentLevel = fuelLevel;
                break;
            case UpgradeType.Speed:
                currentLevel = speedLevel;
                break;
            case UpgradeType.Bonus:
                currentLevel = bonusLevel;
                break;
            default:
                return false;
        }

        if (currentLevel >= currentStage * 10)
        {
            return false;
        }

        //素材チェック
        var (materialId, materialAmount) = GetUpgradeMaterials(type);
        if (materialAmount > 0)
        {
            long owned = GetItemCount(materialId);
            if (owned < materialAmount)
            {
                return false;
            }
        }

        return true;
    }

    public bool CanAdvanceStage()
    {
        //各レベル10より大きければstage上昇可能
        if (storageLevel >= currentStage * 10 && fuelLevel >= currentStage * 10 && speedLevel >= currentStage * 10 && bonusLevel >= currentStage * 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AdvanceStage()
    {
        if (CanAdvanceStage())
        {
            currentStage += 1;
            //画面更新・新イベント用意したほうがきれい
            OnRankChanged?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryUpgrade(UpgradeType type)
    {
        if (!CanUpgrade(type))
        {
            return false;
        }
        long cost = GetUpgradeCost(type);
        if (!SpendGold(cost))
        {
            return false;
        }
        var (materialId, materialAmount) = GetUpgradeMaterials(type);
        if (materialAmount > 0)
        {
            itemCounts[materialId] -= materialAmount;
        }

        switch (type) {
            case UpgradeType.Storage:
                storageLevel++;
                break;
            case UpgradeType.Fuel:
                fuelLevel++;
                break;
            case UpgradeType.Speed:
                speedLevel++;
                break;
            case UpgradeType.Bonus:
                bonusLevel++;
                break;
            default:
                return false;
        }


        OnRankChanged?.Invoke();
        return true;
    }


    //item売却処理
    public bool SellItem(ItemData item)
    {
        if (!item.CanSell)
        {
            return false;
        }
        if (!itemCounts.ContainsKey(item.ItemId))
        {
            return false;
        }
        //キープ数取得
        long keep = GetKeepCount(item.ItemId);

        long canSellCounts = Math.Max(0, itemCounts[item.ItemId] - keep);
        if (canSellCounts <= 0)
        {
            return false;
        }

        AddGold(canSellCounts * item.Value);
        itemCounts[item.ItemId] -= canSellCounts;
        return true;
    }

    public void SellALLItems(List<ItemData> allItems)
    {
        foreach (ItemData item in allItems)
        {
            SellItem(item);
        }
    }

    ////test
    //[SerializeField] private ItemData testItem;

    //[ContextMenu("Test Sell")]
    //public void TestSell()
    //{
    //    bool success = SellItem(testItem);
    //    if (success)
    //    {
    //        Debug.Log($"売却成功：現在Gold {Gold}");
    //    }
    //    else
    //    {
    //        Debug.Log("売却できませんでした（売却不可、未所持、またはキープ数超過）");
    //    }
    //}

    //[ContextMenu("Test Refuel Full")]
    //public void TestRefuelFull()
    //{
    //    long requested = (long)Mathf.Ceil((180 - RemainingFuelLiters));
    //    var (success, affordable) = TryRefuel(requested);
    //    if (success)
    //    {
    //        Debug.Log($"補給成功：Fuel {RemainingFuelLiters}/180L, Gold {Gold}");
    //    }
    //    else
    //    {
    //        Debug.Log($"Gold不足。{affordable}Lまでなら補給可能です");
    //    }
    //}
    //==== 購入テスト用（提出前に削除）====
    [ContextMenu("Test Add Gold 10000")]
    public void TestAddGold()
    {
        AddGold(10000);
        Debug.Log($"Gold追加：現在Gold {Gold}");
    }

    [ContextMenu("Test Purchase Wood x5")]
    public void TestPurchaseWood()
    {
        long before = Gold;
        bool success = TryPurchaseItem("wood", 5);
        Debug.Log($"購入{(success ? "成功" : "失敗")}：Gold {before}→{Gold} / wood所持 {GetItemCount("wood")} / 残り購入可能 {GetRemainingPurchaseCount("wood")}");
    }

    [ContextMenu("Test Expire Wood Window")]
    public void TestExpireWoodWindow()
    {
        //開始時刻を24時間前にずらして、リセットを擬似的に起こす
        if (purchaseWindowStarts.ContainsKey("wood"))
        {
            purchaseWindowStarts["wood"] -= PURCHASE_RESET_SECONDS;
        }
        Debug.Log($"wood残り購入可能 {GetRemainingPurchaseCount("wood")}");
    }

    public void SetKeepCounts(string itemId, long count)
    {
        keepCounts[itemId] = Math.Max(0L, count);
    }

    public void SetShipName(string newName)
    {
        shipName = newName;
        OnShipNameChanged?.Invoke();
    }

    public void SetNotificationEnabled(bool value)
    {
        notificationEnabled = value;
    }

    public void ToggleSailing()
    {
        isSailing = !isSailing;
        OnSailingChanged?.Invoke();
    }

    private void RefreshPurchaseWindow(string itemId)
    {
        if (!purchaseWindowStarts.TryGetValue(itemId, out long start))
        {
            return;
        }

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (now - start >= PURCHASE_RESET_SECONDS)
        {
            purchasedCounts[itemId] = 0;
            purchaseWindowStarts.Remove(itemId);
        }
    }

    public long GetPurchasePrice(ItemData item)
    {
        double rawPrice = item.Value * PURCHASE_PRICE_RATE;
        return (long)Math.Ceiling(rawPrice);
    }

    public long GetRemainingPurchaseCount(string itemId)
    {
        RefreshPurchaseWindow(itemId);
        purchasedCounts.TryGetValue(itemId, out long purchased);

        return Math.Max(0L, DAILY_PURCHASE_LIMIT - purchased);
    }

    public bool TryPurchaseItem(string itemId, long amount)
    {
        ItemData item = GetItemData(itemId);
        long remaining = GetRemainingPurchaseCount(itemId);

        //数量指定不足e
        if (amount <= 0)
        {
            return false;
        }
        //アイテム参照不可e
        if (item == null)
        {
            return false;
        }
        //指定数量超過e
        if (amount > remaining)
        {
            return false;
        }
        //Gold不足e
        long totalCost = GetPurchasePrice(item) * amount;
        if (!SpendGold(totalCost))
        {
            return false;
        }

        AddItem(item, amount);
        //リセット時間設定
        if (!purchaseWindowStarts.ContainsKey(itemId))
        {
            purchaseWindowStarts[itemId] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        purchasedCounts.TryGetValue(itemId, out long purchased);
        purchasedCounts[itemId] = purchased + amount;

        return true;
    }

    //save
    public SaveData GetSaveData()
    {
        SaveData data = new SaveData();
        data.gold = gold;
        data.storageLevel = storageLevel;
        data.fuelLevel = fuelLevel;
        data.speedLevel = speedLevel;
        data.bonusLevel = bonusLevel;
        data.currentStage = currentStage;
        data.remainingFuelLiters = remainingFuelLiters;
        data.isSailing = isSailing;

        data.itemCounts = new List<SaveData.ItemEntry>();
        foreach(var kvp in itemCounts)
        {
            SaveData.ItemEntry entry = new SaveData.ItemEntry();
            entry.key = kvp.Key;
            entry.value = kvp.Value;
            data.itemCounts.Add(entry);
        }

        data.keepCounts = new List<SaveData.ItemEntry>();
        foreach(var kvp in keepCounts)
        {
            SaveData.ItemEntry entry = new SaveData.ItemEntry();
            entry.key = kvp.Key;
            entry.value = kvp.Value;
            data.keepCounts.Add(entry);
        }

        data.purchasedCounts = new List<SaveData.ItemEntry>();
        foreach(var kvp in purchasedCounts)
        {
            SaveData.ItemEntry entry = new SaveData.ItemEntry();
            entry.key = kvp.Key;
            entry.value = kvp.Value;
            data.purchasedCounts.Add(entry);
        }

        data.purchaseWindowStarts = new List<SaveData.ItemEntry>();
        foreach (var kvp in purchaseWindowStarts)
        {
            SaveData.ItemEntry entry = new SaveData.ItemEntry();
            entry.key = kvp.Key;
            entry.value = kvp.Value;
            data.purchaseWindowStarts.Add(entry);
        }

        data.lastSaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return data;
    }

    public void LoadFromSaveData(SaveData data)
    {
        gold = data.gold;
        storageLevel = data.storageLevel;
        fuelLevel = data.fuelLevel;
        speedLevel = data.speedLevel;
        bonusLevel = data.bonusLevel;
        currentStage = data.currentStage;
        remainingFuelLiters = data.remainingFuelLiters;
        isSailing = data.isSailing;

        itemCounts.Clear();
        foreach (var entry in data.itemCounts)
        {
            itemCounts[entry.key] = entry.value;
        }

        keepCounts.Clear();
        foreach (var entry in data.keepCounts)
        {
            keepCounts[entry.key] = entry.value;
        }

        purchasedCounts.Clear();
        if (data.purchasedCounts != null)
        {
            foreach (var entry in data.purchasedCounts)
            {
                purchasedCounts[entry.key] = entry.value;
            }
        }
        

        purchaseWindowStarts.Clear();
        if (data.purchaseWindowStarts != null)
        {
            foreach (var entry in data.purchaseWindowStarts)
            {
                purchaseWindowStarts[entry.key] = entry.value;
            }
        }

        OnGoldChanged?.Invoke();
        OnRankChanged?.Invoke();
        OnFuelChanged?.Invoke();
        OnSailingChanged?.Invoke();
    }

    private IEnumerator AutoSaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            SaveSystem.Save(this);
        }
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save(this);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSystem.Save(this);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveSystem.Save(this);
        }
    }

    public void ResetData()
    {
        gold = 0;
        storageLevel = 0;
        fuelLevel = 0;
        speedLevel = 0;
        bonusLevel = 0;
        currentStage = 1;
        shipName = "ゴーゴー";
        itemCounts.Clear();

        OnGoldChanged?.Invoke();
        OnShipNameChanged?.Invoke();
        OnRankChanged?.Invoke();
    }

}
