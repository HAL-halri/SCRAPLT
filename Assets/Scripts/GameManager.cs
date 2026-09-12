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
    [SerializeField] Dictionary<string, long> reservedCounts = new Dictionary<string, long>();
    [SerializeField] private string shipName = "ゴーゴー";
    [SerializeField] private bool notificationEnabled = true;

    public int SpeedLevel => speedLevel;
    public int BonusLevel => bonusLevel;

    public int CurrentStage => currentStage;
    public long Gold => gold;
    public string ShipName => shipName;
    public bool NotificationEnabled => notificationEnabled;
    public int Rank => storageLevel + fuelLevel + speedLevel+ bonusLevel;

    public float RemainingFuelLiters
    {
        get => remainingFuelLiters;
        private set
        {
            remainingFuelLiters = value;
            OnFuelChanged?.Invoke();
        }
    }

    public void Awake()
    {
        //既に存在している場合、自身を破壊
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ShipStageData GetCurrentStageData()
    {
        //リストから1つずつ要素を取り出し、StageカラムがcurrentStageと一致するものだけを取り出す(LINQ機能)
        return allStages.Find(s => s.Stage == currentStage);
    }

 

    public event Action OnFuelChanged;

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

    //データ増加
    public (bool success, long affordableLiters) TryRefuel(long requestedLiters)
    {
        if(requestedLiters <= 0)
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
        if(Gold >= amount)
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
        if (!reservedCounts.ContainsKey(item.ItemId))
        {
            reservedCounts[item.ItemId] = item.DefaultKeepCount;
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
        if (!reservedCounts.ContainsKey(itemId))
        {
            return 0;
        }
        return reservedCounts[itemId];
    }

    public List<ItemData> GetAvailableItems()
    {
        //取得したものを保存しておく
        HashSet<string> addedIds = new HashSet<string>();
        //戻り値用　現ステージで入手可能なアイテム
        List<ItemData> result = new List<ItemData>();
        foreach (var stage in allStages)
        {
            if(stage.Stage > currentStage)
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

        return currentLevel < currentStage * 10;
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
        if(CanAdvanceStage())
        {
            currentStage += 1;
            return true;
        }
        else
        {
            return false;
        }
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
        if(canSellCounts <= 0)
        {
            return false;
        }

        AddGold(canSellCounts * item.Value);
        itemCounts[item.ItemId] -= canSellCounts;
        return true;
    }

    public void SellALLItems(List<ItemData> allItems)
    {
        foreach(ItemData item in allItems)
        {
            SellItem(item);
        }
    }

    //test
    [SerializeField] private ItemData testItem;

    [ContextMenu("Test Sell")]
    public void TestSell()
    {
        bool success = SellItem(testItem);
        if (success)
        {
            Debug.Log($"売却成功：現在Gold {Gold}");
        }
        else
        {
            Debug.Log("売却できませんでした（売却不可、未所持、またはキープ数超過）");
        }
    }

    [ContextMenu("Test Refuel Full")]
    public void TestRefuelFull()
    {
        long requested = (long)Mathf.Ceil((180 - RemainingFuelLiters));
        var (success, affordable) = TryRefuel(requested);
        if (success)
        {
            Debug.Log($"補給成功：Fuel {RemainingFuelLiters}/180L, Gold {Gold}");
        }
        else
        {
            Debug.Log($"Gold不足。{affordable}Lまでなら補給可能です");
        }
    }

    public void SetKeepCounts(string itemId, long count)
    {
        reservedCounts[itemId] = Math.Max(0L, count);
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
