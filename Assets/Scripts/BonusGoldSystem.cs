using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGoldSystem : MonoBehaviour
{

    [SerializeField] private float intervalSeconds = 15f;
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
        Debug.Log($"{bonusGold}G’Ç‰ÁŠl“¾!");
    }
}
