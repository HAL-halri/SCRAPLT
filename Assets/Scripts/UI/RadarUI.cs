using UnityEngine;
using UnityEngine.UI;

public class RadarUI : MonoBehaviour
{
    [SerializeField] private RectTransform radarCircle;
    [SerializeField] private RectTransform radarLight;
    [SerializeField] private Image radarLightImage;

    [SerializeField] private float radarRadius = 100f;
    [SerializeField] private float minDistance = 0f;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private float minBlinkPeriod = 0.2f;
    [SerializeField] private float maxBlinkPeriod = 1.5f;

    private float currentBlinkPeriod = 1f;
    private bool hasSignal = false;

    public void ShowSignal(Vector2 direction, float distance)
    {
        float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        // Œõ‚ÌˆÊ’uF•ûŒü ~ ‰~‚Ì”¼Œa ~ ‹——£”ä—¦
        radarLight.anchoredPosition = direction * radarRadius * normalizedDistance;

        // “_–ÅüŠúF‹——£‚É‰‚¶‚Ä
        currentBlinkPeriod = Mathf.Lerp(minBlinkPeriod, maxBlinkPeriod, normalizedDistance);

        hasSignal = true;
    }

    private void Update()
    {
        if (!hasSignal) return;

        float alpha = Mathf.PingPong(Time.time, currentBlinkPeriod) / currentBlinkPeriod;
        Color c = radarLightImage.color;
        c.a = alpha;
        radarLightImage.color = c;
    }
}