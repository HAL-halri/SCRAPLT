using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource portSESource;
    [SerializeField] private AudioSource shipSESource;
    [SerializeField] private AudioSource creakSESource;
    [SerializeField] private AudioSource oneShotSource;

    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip portLoop;
    [SerializeField] private AudioClip shipLoop;
    [SerializeField] private AudioClip creakSE;
    [SerializeField] private AudioClip sceneChangeSE;

    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float defaultVolume = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVolume;
    }

    void Start()
    {
        GameManager.Instance.OnSailingChanged += UpdateSailingSE;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSailingChanged -= UpdateSailingSE;
        }
    }

    //----- タイトルBGM -----
    public void PlayTitleBGM()
    {
        bgmSource.clip = titleBGM;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();
        StartCoroutine(FadeAudioSource(bgmSource, 0f, defaultVolume, fadeDuration));
    }

    public void StopTitleBGMWithFade()
    {
        StartCoroutine(StopLoopWithFade(bgmSource));
    }

    //----- 単発SE（タップ時のSceneChange） -----
    public void PlaySceneChangeSE()
    {
        oneShotSource.PlayOneShot(sceneChangeSE);
    }

    //----- 本編の環境音開始（タイトルから遷移時に呼ぶ） -----
    public void StartEnvironmentSE()
    {
        if (GameManager.Instance.IsSailing)
        {
            StartLoopWithFade(shipSESource, shipLoop);
            StartLoopWithFade(creakSESource, creakSE);
        }
        else
        {
            StartLoopWithFade(portSESource, portLoop);
        }
    }

    //----- 停泊/航海切替時（OnSailingChangedで呼ばれる） -----
    private void UpdateSailingSE()
    {
        if (GameManager.Instance.IsSailing)
        {
            StartCoroutine(StopLoopWithFade(portSESource));
            StartLoopWithFade(shipSESource, shipLoop);
            StartLoopWithFade(creakSESource, creakSE);
        }
        else
        {
            StartCoroutine(StopLoopWithFade(shipSESource));
            StartCoroutine(StopLoopWithFade(creakSESource));
            StartLoopWithFade(portSESource, portLoop);
        }
    }

    //----- フェード系ヘルパー -----
    private void StartLoopWithFade(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = true;
        source.volume = 0f;
        source.Play();
        StartCoroutine(FadeAudioSource(source, 0f, defaultVolume, fadeDuration));
    }

    private IEnumerator StopLoopWithFade(AudioSource source)
    {
        yield return FadeAudioSource(source, source.volume, 0f, fadeDuration);
        source.Stop();
    }

    private IEnumerator FadeAudioSource(AudioSource source, float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }
        source.volume = endVolume;
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 1f);
    }
}