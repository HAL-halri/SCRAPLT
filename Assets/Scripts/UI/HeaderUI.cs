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
        GameManager.Instance.OnGoldChanged += UpdateGoldText;
        GameManager.Instance.OnRankChanged += UpdateRankText;
        GameManager.Instance.OnShipNameChanged += UpdateShipNameText;

        UpdateGoldText();
        UpdateRankText();
        UpdateShipNameText();

    }

    void OnDestroy()
    {
        GameManager.Instance.OnGoldChanged -= UpdateGoldText;
        GameManager.Instance.OnRankChanged -= UpdateRankText;
        GameManager.Instance.OnShipNameChanged -= UpdateShipNameText;

    }

    // Update is called once per frame
    void Update()
    {
        fuelText.text = $"Fuel: {GameManager.Instance.GetCurrentFuelMinutes():F0}ï™";
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
        shipNameText.text = $"{GameManager.Instance.ShipName}çÜ";
    }
}
