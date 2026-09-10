using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PortUI : MonoBehaviour
{
    [SerializeField] private TMP_Text fuelText;

    void Start()
    {
        GameManager.Instance.OnFuelChanged += UpdateFuelText;
        UpdateFuelText();
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
}

