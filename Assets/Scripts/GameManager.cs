using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private int efficiencyLevel = 0;
    [SerializeField] private List<ShipStageData> allStages;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private long gold = 0;
    [SerializeField] Dictionary<string, long> itemCounts = new Dictionary<string, long>();
    [SerializeField] private string shipName = "ゴーゴー";
    [SerializeField] private bool notificationEnabled = true;

    public long Gold => gold;
    public int CurrentStage => currentStage;
    public string ShipName => shipName;
    public int EfficiencyLevel => efficiencyLevel;
    public bool NotificationEnabled => notificationEnabled;
    public int Rank => storageLevel + fuelLevel + efficiencyLevel;

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

    public void AddGold(long amount)
    {
        gold += amount;
        //?:nullでなければ変更を知らせる
        OnGoldChanged?.Invoke();
    }

    public void AddItem(string itemId, long amount)
    {
        if (!itemCounts.ContainsKey(itemId))
        {
            itemCounts[itemId] = 0;
        }
        itemCounts[itemId] += amount;
    }

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

    public float GetCurrentFuelMinutes()
    {
        ShipStageData currentData = GetCurrentStageData();

        //Stage内レベルを取得 1レベルごとに20分ずつ増加
        int relativeLevel = fuelLevel - (currentStage - 1) * 10;

        if (relativeLevel >= 10)
        {
            ShipStageData nextData = allStages.Find(s => s.Stage == currentStage + 1);
            if (nextData == null)
            {
                //Stage上限用
                return currentData.FuelDuration * 60f + 9 * 20f;
            }
            return nextData.FuelDuration * 60f;
        }
        else
        {
            return currentData.FuelDuration * 60f + relativeLevel * 20f;
        }
    }

    public float GetIntervalReductionPercent()
    {
        //1レベルごとに3%ずつ短縮　最大30%
        return Mathf.Min(efficiencyLevel, 10) * 3f;
    }

    public float GetNormalBonusPercent()
    {
        //1レベルごとに1%ずつ上昇　10レベル以降から効果発揮
        return Mathf.Min(Mathf.Max(0, efficiencyLevel - 10) * 1f , 40f);
    }

    public float GetBigBonusPercent()
    {
        //10レベルごとに1%ずつ上昇　20レベル以降から効果発揮
        return Mathf.Min(Mathf.Floor(Mathf.Max(0, efficiencyLevel - 10) / 10f) * 1f , 10f);
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
        efficiencyLevel = 0;
        currentStage = 1;
        shipName = "ゴーゴー";
        itemCounts.Clear();

        OnGoldChanged?.Invoke();
        OnShipNameChanged?.Invoke();
        OnRankChanged?.Invoke();
    }

}
