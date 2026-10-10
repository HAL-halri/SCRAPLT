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
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeValueText;

    void OnEnable()
    {
        float currentVolume = BGMManager.Instance.GetMasterVolume();
        volumeSlider.value = currentVolume;
        UpdateVolumeText(currentVolume);
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
    }

    private void OnVolumeSliderChanged(float value)
    {
        BGMManager.Instance.SetMasterVolume(value);
        UpdateVolumeText(value);
    }

    private void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        volumeValueText.text = $"{percent}%";
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
