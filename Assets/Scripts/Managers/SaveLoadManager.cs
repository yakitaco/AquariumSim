using System.IO;
using UnityEngine;

// データの保存と読み込み
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    
    private string saveFilePath;
    public GameSaveData CurrentSaveData { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        // 保存先パス設定
        saveFilePath = Path.Combine(Application.persistentDataPath, "aquarium_save.json");
    }

    // データの読み込み
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            CurrentSaveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("セーブデータを読み込みました。");
        }
        else
        {
            // セーブデータがない場合は新規作成
            CurrentSaveData = new GameSaveData();
            CurrentSaveData.lastSaveTime = System.DateTime.Now.ToString("o");
            Debug.Log("新規セーブデータを作成しました。");
        }
    }

    // データの保存
    public void SaveGame()
    {
        if (CurrentSaveData == null) return;

        // セーブ時刻を現在時刻に更新
        CurrentSaveData.lastSaveTime = System.DateTime.Now.ToString("o");

        // 現在の水質や魚のステータスをCurrentSaveDataに格納する
        // GatherCurrentGameState(); 

        string json = JsonUtility.ToJson(CurrentSaveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("ゲームを保存しました。");
    }
}