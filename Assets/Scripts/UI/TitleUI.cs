using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private TMP_Text tapToStartText;
    [SerializeField] private float blinkDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Button fullScreenButton;

    void OnEnable()
    {
        StartCoroutine(BlinkTextLoop());
        BGMManager.Instance.PlayTitleBGM();
    }

    private IEnumerator BlinkTextLoop()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time, blinkDuration) / blinkDuration;
            tapToStartText.alpha = alpha;
            yield return null;
        }
    }

    public void OnFullScreenButtonClicked()
    {
        fullScreenButton.interactable = false;
        BGMManager.Instance.PlaySceneChangeSE();
        BGMManager.Instance.StopTitleBGMWithFade();
        FadeManager.Instance.StartFadeInOut(fadeDuration, () =>
        {
            titlePanel.SetActive(false);
            BGMManager.Instance.StartEnvironmentSE();
        });
    }
}