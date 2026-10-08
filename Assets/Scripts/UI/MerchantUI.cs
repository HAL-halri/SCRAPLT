using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resetTimeText;
    [SerializeField] private GameObject merchantPanel;
    
    void OnEnable()
    {
        StartCoroutine(UpdateResetTimeLoop());
    }

    private IEnumerator UpdateResetTimeLoop()
    {
        while (true)
        {
            UpdateResetTime();
            yield return new WaitForSeconds(60f);
        }
    }

    private void UpdateResetTime()
    {
        DateTime now = DateTime.Now;
        DateTime nextReset = now.Date.AddDays(1);

        TimeSpan untilReset = nextReset - now;

        int hours = untilReset.Hours;
        int minutes = untilReset.Minutes;

        resetTimeText.text = $"çwì¸êßå¿ÉäÉZÉbÉgÇ‹Ç≈:Ç†Ç∆{untilReset:hh\\mm}";
    }

    public void OnCloseButtonClicked()
    {
        merchantPanel.SetActive(false);
    }
}
