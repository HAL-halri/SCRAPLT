using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "GameData/ItemData")]

public class ItemData : ScriptableObject
{
    //Serial化:インスペクターからは編集可能
    //priveteで外からは編集不可に
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool canSell = true;
    [SerializeField] private long value;
    [SerializeField] private long defaultKeepCount = 10;
    [TextArea, SerializeField] private string description;
    [TextArea, SerializeField] private string obtainMethod;
    [TextArea, SerializeField] private string useDescription;


    //参照だけできるItemIdを用意し、itemIdの中身を引き継ぐ
    public string ItemId => itemId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public bool CanSell => canSell;
    public long Value => value;
    public long DefaultKeepCount => defaultKeepCount;
    public string Description => description;
    public string ObtainMethod => obtainMethod;
    public string UseDescription => useDescription;
}
