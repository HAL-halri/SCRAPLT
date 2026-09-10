using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseUI : MonoBehaviour
{
    [SerializeField] private List<ItemData> displayOrder;
    [SerializeField] private Transform listParent;
    [SerializeFiled] private GameObject itemSlotPrefab;

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
        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        text.text = $"{item.DisplayName}\n{count}";
    }
}
