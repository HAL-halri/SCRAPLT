using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGoldSystem : MonoBehaviour
{

    [SerializeField] private float intervalSeconds = 15f;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private int coinSpawnCount = 8;
    float elapsedSinceLastBonus;

    void Update()
    {
        if (!GameManager.Instance.IsSailing)
        {
            return;
        }

        if (GameManager.Instance.RemainingFuelLiters <= 0)
        {
            return;
        }
        elapsedSinceLastBonus += Time.deltaTime;
        
        if (elapsedSinceLastBonus >= intervalSeconds)
        {
            elapsedSinceLastBonus -= intervalSeconds;
            GrantBonusGold();
        }
    }

    public void GrantBonusGold()
    {
        var (min, max) = GameManager.Instance.GetBonusGoldRange();
        int bonusGold = Random.Range(min, max + 1);
        GameManager.Instance.AddGold(bonusGold);

        bool isExploring = SeabedExplorationSystem.Instance != null && SeabedExplorationSystem.Instance.IsExploring;
        if (!isExploring)
        {
            if (coinSprite != null && FlyingObjectSystem.Instance != null)
            {
                FlyingObjectSystem.Instance.SpawnFlyingObjects(coinSprite, Vector2.zero, coinSpawnCount);
            }
            BGMManager.Instance.PlayCoinSE();
        }

        Debug.Log($"{bonusGold}G’Ç‰ÁŠl“¾!");
    }
}
