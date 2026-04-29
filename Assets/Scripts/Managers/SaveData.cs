using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public string lastSaveTime; // 最後にアプリを閉じた時間（ISO 8601形式の文字列）
    public float waterPollution; // 水の汚れ
    public float waterTemperature; // 水温
    public List<FishSaveData> fishList = new List<FishSaveData>(); // 水槽にいる魚のリスト
}

[Serializable]
public class FishSaveData
{
    public string instanceId; // 個体識別ID
    public string speciesId; // 魚の種類ID
    public float hunger; // 空腹度
    public float age; // 年齢
    public bool isAlive; // 生死
    // 座標なども保存したい場合はここに追加
}