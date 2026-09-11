using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class OptionUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button saveButton;
    [SerializeField] private GameObject resetConfirmPanel;
    [SerializeField] private GameObject optionPanel;

    void OnEnable()
    {
        nameInputField.text = GameManager.Instance.ShipName;

    }

    void Start()
    {
        //buttonが押されたらGMのSetShipNameを呼び出し&入力された名前に変更を実行
        saveButton.onClick.AddListener(() => GameManager.Instance.SetShipName(nameInputField.text));
    }
    //排他制御の実装が必要(他パネルを閉じる)->新設PanelManagerで制御
    //public void ToggleOptionPanel()
    //{
    //    optionPanel.SetActive(!optionPanel.activeSelf);
    //}
}
