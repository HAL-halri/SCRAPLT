using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public long gold;
    public int storageLevel;
    public int fuelLevel;
    public int speedLevel;
    public int bonusLevel;
    public int currentStage;
    public float remainingFuelLiters;
    public bool isSailing;
    public List<ItemEntry> itemCounts;
    public List<ItemEntry> keepCounts;
    public long lastSaveTime;

    [System.Serializable]
    public class ItemEntry
    {
        public string key;
        public long value;
    }
}
