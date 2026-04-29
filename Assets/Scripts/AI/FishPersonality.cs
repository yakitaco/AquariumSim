using System;
using UnityEngine;

[Serializable]
public class FishPersonality
{
    [Tooltip("活発さ：移動頻度や速度の補正")]
    [Range(0.5f, 1.5f)] public float activityLevel = 1.0f;

    [Tooltip("好奇心：餌を見つける索敵範囲の広さ")]
    [Range(0.5f, 1.5f)] public float curiosity = 1.0f;

    [Tooltip("社交性：他個体との距離感（群れるか、単独か）")]
    [Range(0.0f, 1.0f)] public float sociability = 0.5f;

    [Tooltip("臆病さ：環境変化やプレイヤーのタップに対する敏感さ")]
    [Range(0.0f, 1.0f)] public float timidness = 0.5f;

    // 個体生成時にランダムな性格を生成するメソッド
    public static FishPersonality GenerateRandomPersonality()
    {
        return new FishPersonality
        {
            activityLevel = UnityEngine.Random.Range(0.8f, 1.2f),
            curiosity = UnityEngine.Random.Range(0.7f, 1.3f),
            sociability = UnityEngine.Random.Range(0.1f, 0.9f),
            timidness = UnityEngine.Random.Range(0.2f, 0.8f)
        };
    }
}