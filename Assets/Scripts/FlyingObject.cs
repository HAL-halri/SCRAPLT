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

        float angle = Random.Range(60f, 120f) * Mathf.Deg2Rad;
        float speed = Random.Range(initialVelocityMin, initialVelocityMax);
        velocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);

        float sign = Random.value < 0.5f ? -1f : 1f;
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax) * sign;

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

        velocity.y -= gravity * Time.deltaTime;

        transform.localPosition += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

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