using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeepSettingUI : MonoBehaviour
{
    [SerializeField] private GameObject keepRowPrefab;
    [SerializeField] private GameObject keepSettingPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private Transform rowsParent;
    private Dictionary<string, TMP_InputField> inputFields = new Dictionary<string, TMP_InputField>();
    private Dictionary<string, long> initialValue = new Dictionary<string, long>();

    void OnEnable()
    {
        foreach (Transform child in rowsParent)
        {
            Destroy(child.gameObject);
        }
        inputFields.Clear();
        initialValue.Clear();

        List<ItemData> Items = GameManager.Instance.GetAvailableItems();

        foreach (var item in Items)
        {
            CreateRow(item);
        }
    }

    public void CreateRow(ItemData item)
    {
        //Prefabコピー作成
        GameObject row = Instantiate(keepRowPrefab, rowsParent);
        //取得
        TMP_Text nameText = row.transform.Find("NameText").GetComponent<TMP_Text>();
        TMP_InputField inputField = row.transform.Find("CountInputField").GetComponent<TMP_InputField>();

        nameText.text = item.DisplayName;
        //現状のキープ数を事前に入力
        long currentKeepCount = GameManager.Instance.GetKeepCount(item.ItemId);
        inputField.text = currentKeepCount.ToString();
        //辞書に登録
        inputFields[item.ItemId] = inputField;
        initialValue[item.ItemId] = currentKeepCount;
    }

    public bool HasUnsavedChanges()
    {
        //inputFieldの中身をkvpにセットごと取り出す
        foreach (var kvp in inputFields)
        {
            string itemId = kvp.Key;
            TMP_InputField inputField = kvp.Value;

            long.TryParse(inputField.text, out long currentValue);

            if (currentValue != initialValue[itemId])
            {
                return true;
            }
        }
        return false;
    }

    public void OnSaveButtonClicked()
    {
        foreach (var kvp in inputFields)
        {
            string itemId = kvp.Key;
            TMP_InputField inputField = kvp.Value;

            long.TryParse(inputField.text, out long currentValue);

            GameManager.Instance.SetKeepCounts(itemId, currentValue);
            initialValue[itemId] = currentValue;
        }
    }

    public void OnCloseButtonClicked()
    {
        if (!HasUnsavedChanges())
        {
            keepSettingPanel.SetActive(false);
        }
        else
        {
            confirmPanel.SetActive(true);
        }
    }

    public void OnConfirmYesButtonClicked()
    {
        confirmPanel.SetActive(false);
        keepSettingPanel.SetActive(false);
    }

    public void OnConfirmNoButtonClicked()
    {
        confirmPanel.SetActive(false);
    }
}
