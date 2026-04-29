using System;
using UnityEngine;

public class FishStatus : MonoBehaviour
{
    public FishData fishData;       // この個体の種類データ
    public string instanceId;       // セーブ/ロード用の個体識別ID

    public float currentHunger { get; private set; } =100;
    public float age { get; private set; }
    public bool isAlive { get; private set; } = true;

    // 死亡時などに他クラスへ通知するイベント
    public event Action<FishStatus> OnDeath;

    private void Start()
    {
        // オフライン時間の計算完了イベントに登録
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnOfflineTimeProcessed += HandleOfflineProgress;
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnOfflineTimeProcessed -= HandleOfflineProgress;
        }
    }

    // 魚の生成時やロード時にデータを流し込む初期化関数
    public void Initialize(string id, FishData data, float startHunger, float startAge)
    {
        instanceId = id;
        fishData = data;
        currentHunger = startHunger;
        age = startAge;
        isAlive = true;
    }

    private void Update()
    {
        if (!isAlive) return;

        // リアルタイム（プレイ中）のステータス更新処理
        ProcessTime(Time.deltaTime);
    }

    // アプリ起動時にTimeManagerから呼ばれる（オフライン経過時間の反映）
    private void HandleOfflineProgress(float secondsPassed)
    {
        if (!isAlive) return;
        ProcessTime(secondsPassed);
    }

    // 時間経過によるステータス変化のコアロジック
    private void ProcessTime(float deltaSeconds)
    {
        // 1. 年齢の加算と寿命判定
        age += deltaSeconds;
        if (age >= fishData.maxLifespan)
        {
            Die("寿命");
            return;
        }

        // 2. 空腹度の減少と餓死判定
        currentHunger -= fishData.hungerDrainRate * deltaSeconds;
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            Die("餓死");
            return;
        }

        // ※ここに水質・水温による健康状態（病気など）の悪化処理を追加していく
    }

    public void Feed(float amount)
    {
        if (!isAlive) return;
        currentHunger = Mathf.Min(currentHunger + amount, fishData.maxHunger);
    }

    private void Die(string reason)
    {
        isAlive = false;
        Debug.Log($"{fishData.speciesName}(ID:{instanceId}) が死んでしまいました。原因: {reason}");
        
        // 物理挙動をいじって「ひっくり返って浮く」などの死体表現を入れる
        transform.Rotate(0, 0, 180); 
        
        OnDeath?.Invoke(this);
    }
}
