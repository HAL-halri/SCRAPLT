using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ConfirmSailingUI : MonoBehaviour
{
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text line1Text;
    [SerializeField] private TMP_Text line2Text;
    [SerializeField] private TMP_Text line3Text;
    [SerializeField] private CollectionSystem collectionSystem;

    private Action onConfirm;

    public void ShowStartSailingDialog(Action onConfirm)
    {
        this.onConfirm = onConfirm;
        titleText.text = "出航しますか？";
        //予想時間計算
        float remainingFuel = GameManager.Instance.RemainingFuelLiters;
        float fuelDurationHours = GameManager.Instance.GetFuelDurationHoursAtLevel(GameManager.Instance.FuelLevel);
        float fuelDurationSeconds = fuelDurationHours * 3600f;
        float consumeRate = 180f / fuelDurationSeconds;
        float fuelTimeLeft = remainingFuel / consumeRate;
        //満タンまでの時間
        float storageLimit = GameManager.Instance.GetCurrentStorageMinutes() * 60f;
        float storageTimeLeft = storageLimit - collectionSystem.ElapsedStorageTime;
        if (storageTimeLeft < 0f) storageTimeLeft = 0f;
        int currentStage = GameManager.Instance.CurrentStage;

        line1Text.text = $"予想航海時間：{FormatTime(fuelTimeLeft)}";
        line2Text.text = $"満タンまで：{FormatTime(storageTimeLeft)}";
        line3Text.text = $"現在のステージ：Stage {currentStage}";
        confirmPanel.SetActive(true);
    }

    public void ShowEndSailingDialog(Action onConfirm)
    {
        this.onConfirm = onConfirm;
        titleText.text = "帰港しますか？";
        line1Text.text = "";
        line2Text.text = "";
        line3Text.text = "";
        confirmPanel.SetActive(true);
    }

    public void OnConfirmButtonClicked()
    {
        confirmPanel.SetActive(false);
        onConfirm?.Invoke();
    }

    public void OnCancelButtonClicked()
    {
        confirmPanel.SetActive(false);
    }

    private string FormatTime(float seconds)
    {
        if (seconds < 0) seconds = 0;
        TimeSpan ts = TimeSpan.FromSeconds(seconds);
        if (ts.TotalHours >= 1)
        {
            return ts.ToString("hh\\:mm\\:ss");
        }
        return ts.ToString("mm\\:ss");
    }
}
