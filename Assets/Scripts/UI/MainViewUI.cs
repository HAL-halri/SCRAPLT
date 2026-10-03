using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainViewUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Sprite portSprite;
    [SerializeField] private Sprite sailingSprite;
    [SerializeField] private TMP_Text subStatusText;
    [SerializeField] private GameObject subStatusObject;
    [SerializeField] private CollectionSystem collectionSystem;

    void Start()
    {
        GameManager.Instance.OnSailingChanged += UpdateDisplay;
        UpdateDisplay();
        StartCoroutine(UpdateSubStatusLoop());
    }

    void OnDisable()
    {
        GameManager.Instance.OnSailingChanged -= UpdateDisplay;
    }

    private IEnumerator UpdateSubStatusLoop()
    {
        while (true)
        {
            UpdateSubStatus();
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateDisplay()
    {
        bool isSailing = GameManager.Instance.IsSailing;

        if (isSailing)
        {
            backgroundImage.sprite = sailingSprite;
            statusText.text = "çqäCíÜ";
            buttonText.text = "ç`Ç…ãAÇÈ";
        }
        else
        {
            backgroundImage.sprite = portSprite;
            statusText.text = "í‚îëíÜ";
            buttonText.text = "çqäCÇ…èoÇÈ";
        }
    }

    public void OnToggleSailingButtonClicked()
    {
        GameManager.Instance.ToggleSailing();
    }

    private void UpdateSubStatus()
    {
        bool isSailing = GameManager.Instance.IsSailing;
        subStatusObject.SetActive(isSailing);

        if (!isSailing)
        {
            return;
        }

        float remainingFuel = GameManager.Instance.RemainingFuelLiters;
        float fuelDurationHours = GameManager.Instance.GetFuelDurationHoursAtLevel(GameManager.Instance.FuelLevel);
        float fuelDurationSeconds = fuelDurationHours * 3600f;
        float consumeRate = 180f / fuelDurationSeconds;
        float fuelTimeLeft = remainingFuel / consumeRate;

        float storageLimit = GameManager.Instance.GetCurrentStorageMinutes() * 60f;
        float storageTimeLeft = storageLimit - collectionSystem.ElapsedStorageTime;

        subStatusText.text = $"îRóøêÿÇÍÇ‹Ç≈ÅF{FormatTime(fuelTimeLeft)}\nëqå…ñûÉ^ÉìÇ‹Ç≈ÅF{FormatTime(storageTimeLeft)}";
    }

    private string FormatTime(float seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }

        TimeSpan ts = TimeSpan.FromSeconds(seconds);
        if (ts.TotalHours >= 1)
        {
            return ts.ToString("hh\\:mm\\:ss");
        }
        return ts.ToString("mm\\:ss");
    }
}
