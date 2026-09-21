using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeRowUI : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradetype;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text compareText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color warningColor = Color.red;


    private int GetCurrentLevel()
    {
        switch (upgradetype)
        {
            case UpgradeType.Storage:
                return GameManager.Instance.StorageLevel;
            case UpgradeType.Fuel:
                return GameManager.Instance.FuelLevel;
            case UpgradeType.Speed:
                return GameManager.Instance.SpeedLevel;
            case UpgradeType.Bonus:
                return GameManager.Instance.BonusLevel;
            default:
                return 0;
        }
    }

    private string GetTitleText()
    {
        switch (upgradetype)
        {
            case UpgradeType.Storage:
                return "ストレージ";
            case UpgradeType.Fuel:
                return "燃料";
            case UpgradeType.Speed:
                return "航海速度";
            case UpgradeType.Bonus:
                return "ボーナス";
            default:
                return "";
        }
    }

    private string GetCompareText()
    {
        int currentLevel = GetCurrentLevel();
        int nextLevel = currentLevel + 1;
        switch (upgradetype)
        {
            case UpgradeType.Storage:
                {
                    float current = GameManager.Instance.GetStorageMinutesAtLevel(currentLevel);
                    float next = GameManager.Instance.GetStorageMinutesAtLevel(nextLevel);
                    return $"{current:F0}分→{next:F0}分";
                }
            case UpgradeType.Fuel:
                {
                    float current = GameManager.Instance.GetFuelDurationHoursAtLevel(currentLevel);
                    float next = GameManager.Instance.GetFuelDurationHoursAtLevel(nextLevel);
                    return $"{current:F1}時間→{next:F1}時間";
                }
            case UpgradeType.Speed:
                {
                    float current = GameManager.Instance.GetIntervalSecondsAtLevel(currentLevel);
                    float next = GameManager.Instance.GetIntervalSecondsAtLevel(nextLevel);
                    return $"{current:F1}秒→{next:F1}秒";
                }
            case UpgradeType.Bonus:
                {
                    var (currentMin, currentMax) = GameManager.Instance.GetBonusGoldRangeAtLevel(currentLevel);
                    var (nextMin, nextMax) = GameManager.Instance.GetBonusGoldRangeAtLevel(nextLevel);
                    return $"{currentMin}〜{currentMax}G → {nextMin}〜{nextMax}G";
                }
            default:
                return "";
        }
    }
}
