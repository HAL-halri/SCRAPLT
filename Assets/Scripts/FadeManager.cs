using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }
    [SerializeField] private Image fadeOverlay;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn(float duration)
    {
        fadeOverlay.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadeOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeOverlay.color = new Color(0f, 0f, 0f, 1f);
    }

    public IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            fadeOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
        fadeOverlay.gameObject.SetActive(false);
    }

    public IEnumerator FadeInOut(float duration, Action onBlackCallback)
    {
        yield return StartCoroutine(FadeIn(duration));
        onBlackCallback?.Invoke();
        yield return StartCoroutine(FadeOut(duration));
    }

    public void StartFadeInOut(float duration, Action onBlackCallback)
    {
        StartCoroutine(FadeInOut(duration, onBlackCallback));
    }
}
