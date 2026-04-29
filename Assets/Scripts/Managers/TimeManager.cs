using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    // オフライン経過時間を通知するイベント
    public event Action<float> OnOfflineTimeProcessed;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // 起動時にGameManagerから呼ばれる
    public void CalculateOfflineProgress(string lastSaveTimeString)
    {
        if (string.IsNullOrEmpty(lastSaveTimeString)) return;

        try
        {
            DateTime lastSaveTime = DateTime.Parse(lastSaveTimeString);
            DateTime currentTime = DateTime.Now;

            // 経過時間を計算
            TimeSpan timePassed = currentTime - lastSaveTime;
            float passedSeconds = (float)timePassed.TotalSeconds;

            Debug.Log($"オフライン経過時間: {passedSeconds}秒");

            // 経過時間が0より大きければ、魚や水質のマネージャーに時間を進めるよう通知
            if (passedSeconds > 0)
            {
                OnOfflineTimeProcessed?.Invoke(passedSeconds);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("時刻のパースに失敗しました: " + e.Message);
        }
    }
}