using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeabedExplorationSystem : MonoBehaviour
{
    public static SeabedExplorationSystem Instance { get; private set; }
    public bool IsExploring { get; private set; }

    [SerializeField] private GameObject explorationPanel;
    [SerializeField] private RectTransform spotParent;
    [SerializeField] private GameObject spotPrefab;
    [SerializeField] private RadarUI radarUI;
    [SerializeField] private ExplorationTutorialUI tutorialUI;
    [SerializeField] private TMP_Text remainingTimeText;

    [SerializeField] private float resetSeconds = 300f;
    [SerializeField] private int spotCountMin = 3;
    [SerializeField] private int spotCountMax = 7;
    [SerializeField] private float hitRadius = 100f;

    [SerializeField] private int goldMin = 50;
    [SerializeField] private int goldMax = 150;
    [SerializeField] private int goldRareMin = 200;
    [SerializeField] private int goldRareMax = 500;

    private float elapsed;
    private List<ExplorationSpot> currentSpots = new List<ExplorationSpot>();
    private bool needsRespawn = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= resetSeconds)
        {
            elapsed = 0f;
            needsRespawn = true;
        }

        UpdateRemainingTime();
    }

    public void OnPanelOpened()
    {
        IsExploring = true;
        BGMManager.Instance.PlayExplorationBGM();
        if (needsRespawn || currentSpots.Count == 0)
        {
            SpawnSpots();
            needsRespawn = false;
        }


        tutorialUI.ShowIfFirstTime();
    }

    public void OnPanelClosed()
    {
        IsExploring = false;
        BGMManager.Instance.StopExplorationBGM();
    }

    private void SpawnSpots()
    {
        foreach (ExplorationSpot spot in currentSpots)
        {
            if (spot != null) Destroy(spot.gameObject);
        }
        currentSpots.Clear();

        int count = Random.Range(spotCountMin, spotCountMax + 1);
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(spotPrefab, spotParent);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                Random.Range(-spotParent.rect.width / 2f, spotParent.rect.width / 2f),
                Random.Range(-spotParent.rect.height / 2f, spotParent.rect.height / 2f)
            );

            ExplorationSpot spotScript = obj.GetComponent<ExplorationSpot>();
            spotScript.Initialize(this);
            currentSpots.Add(spotScript);
        }
    }

    public void OnAreaClicked(Vector2 clickPos)
    {
        if (currentSpots.Count == 0) return;

        ExplorationSpot nearestSpot = null;
        float nearestDistance = float.MaxValue;

        foreach (ExplorationSpot spot in currentSpots)
        {
            if (spot == null) continue;
            float distance = Vector2.Distance(clickPos, spot.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSpot = spot;
            }
        }

        if (nearestSpot == null) return;

        if (nearestDistance <= hitRadius)
        {
            GenerateReward();
            currentSpots.Remove(nearestSpot);
            Destroy(nearestSpot.gameObject);
        }
        else
        {
            Vector2 direction = (nearestSpot.Position - clickPos).normalized;
            radarUI.ShowSignal(direction, nearestDistance);
        }
    }

    private void GenerateReward()
    {
        float roll = Random.Range(0f, 100f);

        if (roll < 40f)
        {
            int gold = Random.Range(goldMin, goldMax + 1);
            GameManager.Instance.AddGold(gold);
            Debug.Log($"’TõFGold +{gold}");
        }
        else if (roll < 65f)
        {
            int gold = Random.Range(goldMin + 20, goldMax + 50);
            GameManager.Instance.AddGold(gold);
            Debug.Log($"’TõFGold +{gold}");
        }
        else if (roll < 70f)
        {
            int gold = Random.Range(goldRareMin, goldRareMax + 1);
            GameManager.Instance.AddGold(gold);
            Debug.Log($"’TõFGold +{gold}");
        }
        else if (roll < 77f)
        {
            AddMaterialReward("wood");
        }
        else if (roll < 84f)
        {
            AddMaterialReward("cloth");
        }
        else if (roll < 89f)
        {
            AddMaterialReward("dirty_oil");
        }
        else if (roll < 94f)
        {
            AddMaterialReward("scrap");
        }
        else if (roll < 97f)
        {
            AddMaterialReward("plastic");
        }
        else if (roll < 99f)
        {
            AddMaterialReward("plastic_bottle");
        }
        else
        {
            AddMaterialReward("bottle");
        }
    }

    private void AddMaterialReward(string itemId)
    {
        ItemData item = GameManager.Instance.GetItemData(itemId);
        if (item == null) return;
        GameManager.Instance.AddItem(item, 1);
        Debug.Log($"’TõF{item.DisplayName} +1");
    }

    private void UpdateRemainingTime()
    {
        float remaining = resetSeconds - elapsed;
        if (remaining < 0f) remaining = 0f;

        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        remainingTimeText.text = $"ŽŸ‚Ì•ó•¨‚Ü‚ÅF{minutes:00}:{seconds:00}";
    }

    public void ShowTutorial()
    {
        tutorialUI.ShowManual();
    }
}