using UnityEngine;

public enum GameState
{
    Initializing, // 初期化中（ロード等）
    Title,        // タイトル画面
    Playing,      // プレイ中（水槽画面）
    Paused        // メニュー開いている時など
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    // ステートが変更されたときに発火するイベント
    public delegate void GameStateChangedHandler(GameState newState);
    public event GameStateChangedHandler OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        ChangeState(GameState.Initializing);
        InitializeGame();
    }

    // ゲーム起動時のフロー
    private void InitializeGame()
    {
        // 1. データをロード
        SaveLoadManager.Instance.LoadGame();

        // 2. ロードしたデータから前回セーブ時刻を取得し、オフライン経過時間を計算
        string lastTime = SaveLoadManager.Instance.CurrentSaveData.lastSaveTime;
        TimeManager.Instance.CalculateOfflineProgress(lastTime);

        // ※ここで魚の生成などのセットアップを行う

        // 3. 準備ができたらプレイ状態へ（タイトル画面を挟む場合はTitleへ）
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        Debug.Log("Game State Changed to: " + newState);
    }

    // スマホでアプリがバックグラウンドに回った時、または復帰した時に呼ばれる
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // バックグラウンドに移行する直前にセーブ
            SaveLoadManager.Instance.SaveGame();
        }
        else
        {
            // 復帰時に再度オフライン時間を計算する（長時間バックグラウンドにいた場合の対応）
            if (SaveLoadManager.Instance.CurrentSaveData != null)
            {
                TimeManager.Instance.CalculateOfflineProgress(SaveLoadManager.Instance.CurrentSaveData.lastSaveTime);
                // 復帰直後に現在時刻でセーブ時刻を上書きしておく
                SaveLoadManager.Instance.CurrentSaveData.lastSaveTime = System.DateTime.Now.ToString("o");
            }
        }
    }

    // アプリ終了時に呼ばれる（エディタのプレイ停止や、PCビルドでの終了時）
    private void OnApplicationQuit()
    {
        SaveLoadManager.Instance.SaveGame();
    }
}