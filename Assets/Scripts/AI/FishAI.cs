using UnityEngine;

public enum FishState
{
    NormalSwim,   // 通常遊泳
    ChasingFood,  // 餌へ向かう
    Breeding,     // 繁殖行動（パートナーへ向かう）
    Weakened,     // 弱っている（動きが鈍い）
    Dead          // 死亡（動かない、浮く）
}

[RequireComponent(typeof(FishStatus), typeof(FishMovement))]
public class BaseFishAI : MonoBehaviour
{
    protected FishState currentState = FishState.NormalSwim;
    protected FishStatus status;
    protected FishMovement movement;
    protected FishBreeding breeding;
    
    public FishPersonality personality;

    // AIが認識しているターゲット（餌やパートナー）
    protected Transform currentTarget;

    protected virtual void Awake()
    {
        status = GetComponent<FishStatus>();
        movement = GetComponent<FishMovement>();
        breeding = GetComponent<FishBreeding>();
        
        // 個体生成時に性格をランダム決定
        personality = FishPersonality.GenerateRandomPersonality();
    }

    protected virtual void Update()
    {
        // 状態の強制上書きチェック
        CheckVitalStates();

        // 状態に応じた行動の実行
        switch (currentState)
        {
            case FishState.NormalSwim:
                UpdateNormalSwim();
                break;
            case FishState.ChasingFood:
                UpdateChasingFood();
                break;
            case FishState.Breeding:
                UpdateBreeding();
                break;
            case FishState.Weakened:
                UpdateWeakened();
                break;
            case FishState.Dead:
                // 死んでいる場合は何もしない
                break;
        }
    }

    // 生死や健康状態によるステートの強制切り替え
    protected virtual void CheckVitalStates()
    {
        if (!status.isAlive)
        {
            currentState = FishState.Dead;
            movement.SetSpeedMultiplier(0f); // 動きを止める
            return;
        }

        // 空腹度が極端に低い、または水質が悪い場合は「弱る」状態へ
        if (status.currentHunger < status.fishData.maxHunger * 0.2f)
        {
            currentState = FishState.Weakened;
            return;
        }

        // 弱り状態から回復した場合の復帰処理
        if (currentState == FishState.Weakened && status.currentHunger >= status.fishData.maxHunger * 0.2f)
        {
            currentState = FishState.NormalSwim;
        }
    }

    // --- 各ステートの具体的な処理（Virtualにして子クラスで変更可能にする） ---

    protected virtual void UpdateNormalSwim()
    {
        // 性格（活発さ）に応じて移動速度を補正
        movement.SetSpeedMultiplier(personality.activityLevel);
        movement.SwimRandomly();

        // 1. 餌を探す（好奇心が高いほど遠くの餌を見つける）
        float searchRadius = 5f * personality.curiosity;
        Transform food = FindClosestFood(searchRadius);
        if (food != null && status.currentHunger < status.fishData.maxHunger * 0.8f)
        {
            currentTarget = food;
            currentState = FishState.ChasingFood;
            return;
        }

        // 2. 繁殖可能かチェックする
        if (breeding != null && breeding.CanBreed())
        {
            Transform mate = breeding.FindMate();
            if (mate != null)
            {
                currentTarget = mate;
                currentState = FishState.Breeding;
            }
        }
    }

    protected virtual void UpdateChasingFood()
    {
        if (currentTarget == null)
        {
            currentState = FishState.NormalSwim;
            return;
        }

        movement.SetSpeedMultiplier(personality.activityLevel * 1.5f); // 餌に向かう時は少し早く
        movement.SwimTowards(currentTarget.position);

        // 餌に十分に近づいた場合（捕食処理は餌側かここで実装）
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            // 食べる処理（擬似コード）
            // status.Feed(currentTarget.GetComponent<Food>().nutrition);
            // Destroy(currentTarget.gameObject);
            currentState = FishState.NormalSwim;
        }
    }

    protected virtual void UpdateBreeding()
    {
        if (currentTarget == null || !breeding.CanBreed())
        {
            currentState = FishState.NormalSwim;
            return;
        }

        movement.SwimTowards(currentTarget.position);

        // パートナーに十分に近づいたら繁殖実行
        if (Vector3.Distance(transform.position, currentTarget.position) < 1.0f)
        {
            breeding.ExecuteBreeding(currentTarget.GetComponent<FishStatus>());
            currentState = FishState.NormalSwim; // 繁殖後は通常に戻る
        }
    }

    protected virtual void UpdateWeakened()
    {
        // 弱っている時は動きを極端に遅くする
        movement.SetSpeedMultiplier(0.3f);
        movement.SwimRandomly();
    }

    // 周囲の餌を探すロジック（擬似）
    protected Transform FindClosestFood(float radius)
    {
        // Physics.OverlapSphereなどで"Food"レイヤーのオブジェクトを探し、最短距離のものを返す
        return null; 
    }
}