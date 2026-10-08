using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailUI : MonoBehaviour
{
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text useText;
    [SerializeField] private TMP_Text obtainText;
    [SerializeField] private TMP_Text descriptionText;

    public void ShowItem(ItemData item)
    {
        if (item.CanSell)
        {
            valueText.text = $"îÑãpÅF{item.Value}G";
        }
        else
        {
            valueText.text = "îÑãp : ïsâ¬";
        }
        iconImage.sprite = item.Icon;
        nameText.text = item.DisplayName;
        useText.text = $"ópìrÅF{item.UseDescription}";
        obtainText.text = $"ì¸éËÅF{item.ObtainMethod}";
        descriptionText.text = item.Description;
        itemDetailPanel.SetActive(true);
    }

    public void OnCloseButtonClicked()
    {
        itemDetailPanel.SetActive(false);
    }
    public void Hide()
    {
        itemDetailPanel.SetActive(false);
    }
}
