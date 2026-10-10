using UnityEngine;
using UnityEngine.UI;

public class FlyingObject : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float initialVelocityMin = 300f;
    [SerializeField] private float initialVelocityMax = 600f;
    [SerializeField] private float gravity = 1500f;
    [SerializeField] private float lifeTime = 1.5f;
    [SerializeField] private float fadeStart = 1.0f;
    [SerializeField] private float rotationSpeedMin = 180f;
    [SerializeField] private float rotationSpeedMax = 540f;

    private Vector2 velocity;
    private float elapsed;
    private float rotationSpeed;

    public void Initialize(Sprite sprite)
    {
        image.sprite = sprite;

        // ランダム初速（上向きに飛ばす：60〜120度）
        float angle = Random.Range(60f, 120f) * Mathf.Deg2Rad;
        float speed = Random.Range(initialVelocityMin, initialVelocityMax);
        velocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);

        // ランダム回転速度（左右どちらにも回る）
        float sign = Random.value < 0.5f ? -1f : 1f;
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax) * sign;

        // 初期状態
        elapsed = 0f;
        Color c = image.color;
        c.a = 1f;
        image.color = c;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // 重力
        velocity.y -= gravity * Time.deltaTime;

        // 移動
        transform.localPosition += (Vector3)(velocity * Time.deltaTime);

        // 回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // フェード
        if (elapsed >= fadeStart)
        {
            float fadeProgress = (elapsed - fadeStart) / (lifeTime - fadeStart);
            float alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
}