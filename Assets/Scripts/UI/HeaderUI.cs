using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeaderUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text shipNameText;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text fuelText;


    // Start is called before the first frame update
    void Start()
    {
        //自動的に更新処理を実行するための登録
        GameManager.Instance.OnGoldChanged += UpdateGoldText;
        GameManager.Instance.OnRankChanged += UpdateRankText;
        GameManager.Instance.OnShipNameChanged += UpdateShipNameText;
        GameManager.Instance.OnFuelChanged += UpdateFuelText;

        //初回更新
        UpdateGoldText();
        UpdateRankText();
        UpdateShipNameText();
        UpdateFuelText();

    }

    void OnDestroy()
    {
        GameManager.Instance.OnGoldChanged -= UpdateGoldText;
        GameManager.Instance.OnRankChanged -= UpdateRankText;
        GameManager.Instance.OnShipNameChanged -= UpdateShipNameText;
        GameManager.Instance.OnFuelChanged -= UpdateFuelText;

    }

    void UpdateGoldText()
    {
        goldText.text = $"Gold: {GameManager.Instance.Gold}";
    }

    void UpdateRankText()
    {
        rankText.text = $"Rank: {GameManager.Instance.Rank}";
    }

    void UpdateShipNameText()
    {
        shipNameText.text = $"{GameManager.Instance.ShipName}号";
    }

    void UpdateFuelText()
    {
        fuelText.text = $"Fuel:{Mathf.FloorToInt(GameManager.Instance.RemainingFuelLiters)}/180L";
    }
}
