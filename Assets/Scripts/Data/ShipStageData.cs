using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レコードを追加するために必要
[CreateAssetMenu(fileName = "NewShipStageData", menuName = "GameData/ShipStageData")]


public class ShipStageData : ScriptableObject
{
    [SerializeField] private int stage;
    [SerializeField] private float storage;
    //[SerializeField] private float fuelDuration;
    [SerializeField] private List<string> unlockedFeatures;
    //カラムの中にDropRateDataテーブルを埋め込むイメージ
    [SerializeField] private List<DropRateData> dropRates;

    public int Stage => stage;
    public float Storage => storage;
    //public float FuelDuration => fuelDuration;
    public List<string> UnlockedFeatures => unlockedFeatures;
    public List<DropRateData> DropRates => dropRates;

}
