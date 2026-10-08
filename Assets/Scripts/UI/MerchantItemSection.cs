using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerchantItemSection : MonoBehaviour
{
    [SerializeField] private string itemId;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Slider amountSlider;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text remainingText;
    [SerializeField] private long incrementAmount = 1;

    void OnEnable()
    {
        GameManager.Instance.OnGoldChanged += RefreshDisplay;
        amountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        RefreshDisplay();
    }

    void OnDisable()
    {
        GameManager.Instance.OnGoldChanged -= RefreshDisplay;
        amountSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    public void RefreshDisplay()
    {
        ItemData item = GameManager.Instance.GetItemData(itemId);
        if (item == null)
        {
            return;
        }
        itemIcon.sprite = item.Icon;
        itemNameText.text = item.DisplayName;
        long price = GameManager.Instance.GetPurchasePrice(item);
        priceText.text = $"{price}G";
        long remaining = GameManager.Instance.GetRemainingPurchaseCount(itemId);
        remainingText.text = $"–{“ú‚ÌŽc‚è:{remaining}ŒÂ";

        long affordable;
        if (price > 0)
        {
            affordable = GameManager.Instance.Gold / price;
        }
        else
        {
            affordable = 0;
        }
            long maxValue = Math.Min(remaining, affordable);
        amountSlider.maxValue = Math.Max(1, maxValue);
        if (amountSlider.value > amountSlider.maxValue)
        {
            amountSlider.value = amountSlider.maxValue;
        }
        if (maxValue > 0)
        {
            purchaseButton.interactable = true;
        }
        else
        {
            purchaseButton.interactable = false;
        }
        OnSliderValueChanged(amountSlider.value);
    }

    private void OnSliderValueChanged(float value)
    {
        amountText.text = $"{(int)value}ŒÂ";
    }

    public void OnPlusButtonClicked()
    {
        float newValue = amountSlider.value + incrementAmount;
        amountSlider.value = Mathf.Clamp(newValue, 0f, amountSlider.maxValue);
    }

    public void OnMinusButtonClicked()
    {
        float newValue = amountSlider.value - incrementAmount;
        amountSlider.value = Mathf.Clamp(newValue, 0f, amountSlider.maxValue);
    }

    public void OnPurchaseButtonClicked()
    {
        long amount = (long)amountSlider.value;
        if (amount <= 0)
        {
            return;
        }

        bool success = GameManager.Instance.TryPurchaseItem(itemId, amount);
        if (success)
        {
            RefreshDisplay();
        }
    }
}
