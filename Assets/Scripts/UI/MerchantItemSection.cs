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
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color warningColor = Color.red;

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
        if (item == null) return;

        itemIcon.sprite = item.Icon;
        itemNameText.text = item.DisplayName;

        long price = GameManager.Instance.GetPurchasePrice(item);
        priceText.text = $"{price}G";

        long remaining = GameManager.Instance.GetRemainingPurchaseCount(itemId);
        remainingText.text = $"本日の残り:{remaining}個";

        // スライダーMax = 残り在庫のみ考慮
        amountSlider.maxValue = Math.Max(1, remaining);
        if (amountSlider.value > amountSlider.maxValue)
        {
            amountSlider.value = amountSlider.maxValue;
        }

        // 購入ボタン：在庫 > 0 かつ 現在の数量で購入可能
        int currentAmount = (int)amountSlider.value;
        long totalCost = price * currentAmount;
        if (remaining > 0 && GameManager.Instance.Gold >= totalCost)
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
        int amount = (int)value;
        amountText.text = $"{amount}個";

        ItemData item = GameManager.Instance.GetItemData(itemId);
        if (item == null) return;

        long price = GameManager.Instance.GetPurchasePrice(item);
        long totalCost = price * amount;

        if (totalCost > GameManager.Instance.Gold)
        {
            amountText.color = warningColor;
        }
        else
        {
            amountText.color = normalColor;
        }
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
