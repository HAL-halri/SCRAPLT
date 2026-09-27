using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentStageText;
    [SerializeField] private Button advanceStageButton;
    [SerializeField] private GameObject warningPopup;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private CanvasGroup warningCanvasGroup;
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine hideWarningCoroutine;

    void OnEnable()
    {
        RefreshStageText();
        GameManager.Instance.OnRankChanged += RefreshStageText;
    }
    
    void OnDisable()
    {
        GameManager.Instance.OnRankChanged -= RefreshStageText;
    }

    private void RefreshStageText()
    {
        int current = GameManager.Instance.CurrentStage;
        int total = GameManager.Instance.TotalStages;

        currentStageText.text = $"Stage {current}/{total}";
    }

    public void OnAdvanceStageButtonClicked()
    {
        if (GameManager.Instance.CanAdvanceStage())
        {
            GameManager.Instance.AdvanceStage();
            RefreshStageText();
        }
        else
        {
            int requiredRank = GameManager.Instance.CurrentStage * 4 * 10;
            ShowWarning($"Rank {requiredRank} 到達後に進行可能です");
        }
    }

    private void ShowWarning(string message)
    {
        warningCanvasGroup.alpha = 1f;
        warningText.text = message;
        warningPopup.SetActive( true );

        if (hideWarningCoroutine != null)
        {
            //前のcoroutineを一時停止
            StopCoroutine(hideWarningCoroutine);
        }
        hideWarningCoroutine = StartCoroutine(HideWarningAfterDelay());
    }

    //coroutineのための戻り値設定
    private IEnumerator HideWarningAfterDelay()
    {
        float elapsed = 0f;
        //3秒間待機 yield return で一時停止&値を返す
        yield return new WaitForSeconds(warningDuration - fadeDuration);
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            warningCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        warningPopup.SetActive(false);
        hideWarningCoroutine = null;
    }
}
