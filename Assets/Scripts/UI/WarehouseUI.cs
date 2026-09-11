using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WarehouseUI : MonoBehaviour
{
    [SerializeField] private GameObject warehousePanel; 
    [SerializeField] private List<ItemData> displayOrder;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject itemSlotPrefab;

    //public void ToggleWarehousePanel()
    //{
    //    warehousePanel.SetActive(!warehousePanel.activeSelf);
    //    if (warehousePanel.activeSelf)
    //    {
    //        RefreshWarehouse();
    //    }
    //}

    void OnEnable()
    {
        RefreshWarehouse();
    }

    public void RefreshWarehouse()
    {
        foreach(Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

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
}
