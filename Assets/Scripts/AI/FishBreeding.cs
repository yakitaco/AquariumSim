using UnityEngine;

[RequireComponent(typeof(FishStatus))]
public class FishBreeding : MonoBehaviour
{
    private FishStatus status;
    
    [Header("Breeding Settings")]
    public float breedingCooldown = 3600f; // 繁殖後、次に繁殖可能になるまでの時間（秒）
    private float currentCooldown = 0f;

    public bool isMale; // 性別

    private void Start()
    {
        status = GetComponent<FishStatus>();
        // 初期化時に性別をランダム決定（半々の確率）
        isMale = UnityEngine.Random.value > 0.5f; 
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    // 自身が繁殖可能な状態か判定する
    public bool CanBreed()
    {
        if (!status.isAlive) return false;
        if (currentCooldown > 0) return false;

        // 1. 年齢チェック（例：寿命の20%〜80%の間だけ繁殖可能）
        float maturityAge = status.fishData.maxLifespan * 0.2f;
        float oldAge = status.fishData.maxLifespan * 0.8f;
        if (status.age < maturityAge || status.age > oldAge) return false;

        // 2. 健康状態チェック（お腹が十分に満たされているか）
        if (status.currentHunger < status.fishData.maxHunger * 0.7f) return false;

        // 3. 環境チェック（水質マネージャーや水温マネージャーが存在すると仮定）
        // ※ Singletonのマネージャーから値を取得
        /*
        if (WaterQualityManager.Instance.currentPollution > 30f) return false; // 水が汚いと繁殖しない
        
        float currentTemp = TemperatureManager.Instance.currentTemp;
        float idealTemp = status.fishData.idealTemperature;
        if (Mathf.Abs(currentTemp - idealTemp) > status.fishData.tempTolerance) return false; // 水温が適正でないと繁殖しない
        */

        return true;
    }

    // 周囲の適切なパートナー（同種、異性、かつ相手も繁殖可能）を探す
    public Transform FindMate()
    {
        // 簡易的な実装：周囲のコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (var hitCollider in hitColliders)
        {
            FishBreeding potentialMate = hitCollider.GetComponent<FishBreeding>();
            
            if (potentialMate != null && potentialMate != this)
            {
                FishStatus mateStatus = potentialMate.GetComponent<FishStatus>();

                // 同種、異性、相手も繁殖可能状態かチェック
                if (mateStatus.fishData.speciesId == status.fishData.speciesId &&
                    potentialMate.isMale != this.isMale &&
                    potentialMate.CanBreed())
                {
                    return potentialMate.transform;
                }
            }
        }
        return null;
    }

    // AIから呼ばれ、実際に稚魚を生成する処理
    public void ExecuteBreeding(FishStatus partnerStatus)
    {
        Debug.Log($"{status.fishData.speciesName} の繁殖が成功しました！");

        // 稚魚（プレハブ）を生成。場所は親の少し横
        Vector3 spawnPos = transform.position + new Vector3(0.5f, 0, 0);
        GameObject fry = Instantiate(status.fishData.prefab, spawnPos, Quaternion.identity);

        // 稚魚の初期化（サイズを小さくし、年齢を0にする）
        fry.transform.localScale = status.fishData.prefab.transform.localScale * 0.3f;
        
        FishStatus fryStatus = fry.GetComponent<FishStatus>();
        string newId = System.Guid.NewGuid().ToString(); // 新しい固有IDを生成
        
        // 稚魚は最初から少しお腹が空いている状態でスタートさせるなど調整可能
        fryStatus.Initialize(newId, status.fishData, status.fishData.maxHunger * 0.5f, 0f);

        // クールダウンを開始（親両方）
        this.currentCooldown = breedingCooldown;
        FishBreeding partnerBreeding = partnerStatus.GetComponent<FishBreeding>();
        if (partnerBreeding != null)
        {
            partnerBreeding.currentCooldown = partnerBreeding.breedingCooldown;
        }

        // ※ 遺伝アルゴリズムを導入する場合、ここで親(this)と(partner)の性格パラメータを
        // 混ぜ合わせた新しいFishPersonalityを稚魚にセットすると、代を重ねるごとに個性が進化します。
    }
}