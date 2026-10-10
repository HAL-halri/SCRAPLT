using UnityEngine;

public class FlyingObjectSystem : MonoBehaviour
{
    public static FlyingObjectSystem Instance { get; private set; }

    [SerializeField] private GameObject flyingObjectPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnRadiusX = 300f;
    [SerializeField] private float spawnRadiusY = 100f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnFlyingObjects(Sprite sprite, Vector2 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(flyingObjectPrefab, spawnParent);
            RectTransform rect = obj.GetComponent<RectTransform>();

            Vector2 offset = new Vector2(
                Random.Range(-spawnRadiusX, spawnRadiusX),
                Random.Range(-spawnRadiusY, spawnRadiusY)
            );
            rect.anchoredPosition = position + offset;

            FlyingObject flying = obj.GetComponent<FlyingObject>();
            flying.Initialize(sprite);
        }
    }
}