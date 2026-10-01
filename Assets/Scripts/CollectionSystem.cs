using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    [SerializeField] private float intervalSeconds = 30f;　//デバッグ用　最終的には60f
    [SerializeField] private float speedMultiplier = 1f; //時間加速倍率


    private float elapsedSinceLastDraw = 0f;
    private float elapsedStorageTime = 0f;


    private void Update()
    {


        ShipStageData stageData = GameManager.Instance.GetCurrentStageData();
        if (stageData == null)
        {
            //NotNull
            return;
        }

        if (!GameManager.Instance.IsSailing)
        {
            return;
        }

        GameManager.Instance.ConsumeFuel(Time.deltaTime / 3600f);

        if (GameManager.Instance.RemainingFuelLiters <= 0f)
        {
            // 収集処理をスキップ
            return;
        }

        float storageLimitSeconds = GameManager.Instance.GetCurrentStorageMinutes() * 60f; //秒単位に変換
        if(elapsedStorageTime >= storageLimitSeconds)
        {
            //満タンならやめる
            return;
        }

        //時間加速想定
        float scaledDelta = Time.deltaTime * speedMultiplier;
        //経過時間
        elapsedSinceLastDraw += scaledDelta;
        //合計時間
        elapsedStorageTime += scaledDelta;

        //効率アップ効果を受けての実際の待ち時間
        float actualInterval = intervalSeconds * (1 - GameManager.Instance.GetIntervalReductionPercent() / 100f);

        //抽選
        if(elapsedSinceLastDraw >= actualInterval)
        {
            //小数点以下のずれを防ぐためにデクリメント処理を行う
            elapsedSinceLastDraw -= actualInterval;
            DrawItem(stageData);
        }


    }

    public void DrawItem(ShipStageData stageData)
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;
        int totalCount =1;

        foreach (DropRateData drop in stageData.DropRates)
        {
            //アイテム種ごとに累積していき、引いた数より累積が大きくなったタイミングのアイテムが当選
            cumulative += drop.probability;
            if (roll <= cumulative)
            {

                if (Random.Range(0f, 100f) <= GameManager.Instance.GetNormalBonusPercent())
                {
                    totalCount += 1;
                }
                if (Random.Range(0f, 100f) <= GameManager.Instance.GetBigBonusPercent())
                {
                    totalCount *= 5;
                }
                GameManager.Instance.AddItem(drop.item, totalCount*999);
                Debug.Log($"Drew: {drop.item} * {totalCount*999} (roll={roll:F1}, cumulative={cumulative})");
                return;
            }
        }
    }

}
