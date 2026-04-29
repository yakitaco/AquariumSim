using UnityEngine;

[CreateAssetMenu(fileName = "NewFishData", menuName = "Aquarium/FishData")]
public class FishData : ScriptableObject
{
    public string speciesId;        // 種類ID（例: "neon_tetra"）
    public string speciesName;      // 表示名
    public GameObject prefab;       // 表示用3Dモデルのプレハブ

    [Header("Life Parameters")]
    public float maxLifespan;       // 寿命（秒単位。例: 1年は 31536000秒）
    public float maxHunger;         // 空腹度の最大値
    public float hungerDrainRate;   // 1秒あたりの空腹減少量

    [Header("Environment Requirements")]
    public float idealTemperature;  // 適正水温
    public float tempTolerance;     // 許容できる水温のブレ幅（適正±この値）

    [Header("Movement")]
    public float swimSpeed;         // 泳ぐスピード
    public float turnSpeed;         // 旋回スピード
}