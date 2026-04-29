using UnityEngine;

[RequireComponent(typeof(FishStatus))]
public class FishMovement : MonoBehaviour
{
    private FishStatus status;
    private Vector3 targetPosition;

    [Header("Obstacle Avoidance")]
    public float detectionDistance = 1.5f; // 障害物を検知する距離
    public LayerMask obstacleMask;         // 壁や障害物のレイヤー

    // 水槽の遊泳可能エリア（本来はTankManager等から動的に取得するのが望ましい）
    private Vector3 swimAreaMin = new Vector3(-4f, -1.5f, -2.5f);
    private Vector3 swimAreaMax = new Vector3(4f, 1.5f, 2.5f);

    private float speedMultiplier = 1.0f; // 速度の倍率（AI・性格から設定）
    private bool isRandomSwimming = true; // 現在ランダム遊泳中かどうか
    private float avoidCooldown = 0f;     // 障害物回避中、一時的にターゲット上書きを無視する時間

    private void Start()
    {
        status = GetComponent<FishStatus>();
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (!status.isAlive) return; // 死んでいたら泳がない

        // 回避行動のクールダウンを減少
        if (avoidCooldown > 0f)
        {
            avoidCooldown -= Time.deltaTime;
        }

        MoveAndAvoid();
    }
    
    public void SwimRandomly()
    {
        isRandomSwimming = true;
        // ターゲット到達時の「次のランダム地点の設定」は MoveAndAvoid 内で自動処理
    }
    
    public void SwimTowards(Vector3 target)
    {
        isRandomSwimming = false;
        
        // 障害物回避のクールダウン中でなければ、ターゲットを更新する
        // （これにより、餌に向かっている最中でも壁や障害物を優先して避けます）
        if (avoidCooldown <= 0f)
        {
            targetPosition = target;
        }
    }

    public void SetSpeedMultiplier(float level)
    {
        speedMultiplier = level;
    }

    private void MoveAndAvoid()
    {
        Transform t = transform;
        Vector3 forward = t.forward;

        // 前方にRayを飛ばして障害物（ガラス面や岩など）を検知
        RaycastHit hit;
        if (Physics.Raycast(t.position, forward, out hit, detectionDistance, obstacleMask))
        {
            // 障害物にぶつかりそうなら、法線ベクトルを元に反射方向を計算
            Vector3 avoidDir = Vector3.Reflect(forward, hit.normal);
            
            // 壁に垂直に当たった際に不自然に反復しないよう、少しランダムに散らす
            avoidDir = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0) * avoidDir;
            
            // 新しい目標地点を回避方向に設定
            targetPosition = t.position + avoidDir.normalized * 2f;
            
            // 1秒間は外部（AI）からのターゲット更新を無視し、回避行動に専念する
            avoidCooldown = 1.0f; 
        }

        // 目標地点への方向ベクトル
        Vector3 directionToTarget = (targetPosition - t.position).normalized;

        // Quaternion.Slerpを使って、現在の向きから目標の向きへ滑らかに回転させる
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            // 旋回速度に speedMultiplier を適用
            t.rotation = Quaternion.Slerp(t.rotation, targetRotation, status.fishData.turnSpeed * speedMultiplier * Time.deltaTime);
        }

        // 前進（移動速度に speedMultiplier を適用）
        t.Translate(Vector3.forward * status.fishData.swimSpeed * speedMultiplier * Time.deltaTime);

        // 目標地点に十分に近づいた場合
        if (Vector3.Distance(t.position, targetPosition) < 0.5f)
        {
            if (isRandomSwimming)
            {
                // ランダム遊泳モードなら新しい目標をセット
            SetNewRandomTarget();
        }
            // 追従モード(isRandomSwimming == false)の場合は、AI側で距離を判定して
            // 食べる/繁殖する等の処理を行うため、ここでは何もしない
        }
    }

    private void SetNewRandomTarget()
    {
        // 指定されたエリア内でランダムな座標を生成
        float x = Random.Range(swimAreaMin.x, swimAreaMax.x);
        float y = Random.Range(swimAreaMin.y, swimAreaMax.y);
        float z = Random.Range(swimAreaMin.z, swimAreaMax.z);
        targetPosition = new Vector3(x, y, z);
    }

    // エディタのSceneビューでRayを可視化（デバッグ用）
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * detectionDistance);
    }
}
