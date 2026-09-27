using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortUI : MonoBehaviour
{
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Slider amountSlider;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button refuelSpecifiedButton;
    [SerializeField] private GameObject refuelConfirmPanel;
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private long incrementAmount = 10;

    void Start()
    {
        GameManager.Instance.OnFuelChanged += UpdateFuelText;
        UpdateFuelText();
        //値が変わったら関数を呼び出す
        amountSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnDestroy()
    {
        GameManager.Instance.OnFuelChanged -= UpdateFuelText;
    }

    void UpdateFuelText()
    {
        fuelText.text = $"{Mathf.FloorToInt(GameManager.Instance.RemainingFuelLiters)}/180L";
    }

    public void OnRefuelButtonClicked()
    {
        long requested = (long)Mathf.Ceil(180 - GameManager.Instance.RemainingFuelLiters);
        var (success, affordable) = GameManager.Instance.TryRefuel(requested);

        if (success)
        {
            Debug.Log($"補給成功：Fuel {GameManager.Instance.RemainingFuelLiters}/180L, Gold {GameManager.Instance.Gold}");
        }
        else
        {
            Debug.Log($"Gold不足。{affordable}Lまでなら補給可能です");
        }
    }

    private void RefreshSliderMax()
    {
        float space = 180 - GameManager.Instance.RemainingFuelLiters;
        amountSlider.maxValue = space;
        if (amountSlider.value > space)
        {
            amountSlider.value = space;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        amountText.text = $"{(int)value}L";
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

    public void OnRefuelSpecifiedButtonClicked()
    {
        long requestedLiters = (long)amountSlider.value;
        if (requestedLiters <= 0)
        {
            return;
        }

        var (success, affordableLiters) = GameManager.Instance.TryRefuel(requestedLiters);

        if (success)
        {
            RefreshSliderMax();
        }
        else
        {
            ShowConfirmPanel(affordableLiters);
        }
    }

    private void ShowConfirmPanel(long affordableLiters)
    {

    }

    public void OnConfirmYesButtonClicked()
    {

    }

    public void OnCloseButtonClicked()
    {

    }
}

