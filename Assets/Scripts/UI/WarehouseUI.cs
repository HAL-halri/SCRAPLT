using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WarehouseUI : MonoBehaviour
{
    [SerializeField] private List<ItemData> displayOrder;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject itemSlotPrefab;


    void OnEnable()
    {
        RefreshWarehouse();
    }

    public void RefreshWarehouse()
    {
        foreach (Transform child in listParent)
        {
            child.gameObject.SetActive(false);
        }
        StartCoroutine(RebuildSlots());
    }

    private IEnumerator RebuildSlots()
    {
        //1フレーム待機しトラッカーの計算時間を確保
        yield return null;

        //1つずつ削除
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in listParent)
        {
            toDestroy.Add(child.gameObject);
        }
        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }

        //1フレーム待機
        yield return null;

        //アイテムスロット更新
        foreach (var item in displayOrder)
        {
            if (!GameManager.Instance.HasItem(item.ItemId))
            {
                continue;
            }

            long count = GameManager.Instance.GetItemCount(item.ItemId);
            int slotCount = (int)Mathf.Ceil((float)count / 999);

            for (int i = 0; i < slotCount; i++)
            {
                long display = Math.Min(999, count - 999 * i);
                CreateSlot(item, display);
            }
        }
    }

    public void CreateSlot(ItemData item, long count)
    {
        //形のコピーを作成
        GameObject slot = Instantiate(itemSlotPrefab, listParent);
        //テキストを探す(画像に差し替え必須)
        TMP_Text name = slot.GetComponent<TMP_Text>();
        TMP_Text countText = slot.transform.Find("CountText").GetComponent<TMP_Text>();

        name.text = $"{item.DisplayName}";
        countText.text = $"{count}";
    }

    public void OnSellAllButoonClicked()
    {
        GameManager.Instance.SellALLItems(displayOrder);
        RefreshWarehouse();
    }
}