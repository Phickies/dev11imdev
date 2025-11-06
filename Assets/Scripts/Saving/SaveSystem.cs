using UnityEngine;
using System.IO;
using Assets.Scripts;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerData PlayerDat;
        public ComboData ComboDat;
        public SceneEnemyData EnemyData;
        public CardData CardDat;

    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
        Debug.Log(SaveFileName());

    }

    private static void HandleSaveData()
    {
        GameManager.Instance.playerManager.Save(ref _saveData.PlayerDat);
        GameManager.Instance.comboMan.Save(ref _saveData.ComboDat);
        GameManager.Instance.spawnManager.Save(ref _saveData.EnemyData);
        GameManager.Instance.cardman.Save(ref _saveData.CardDat);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GameManager.Instance.playerManager.Load(_saveData.PlayerDat);
        GameManager.Instance.comboMan.Load(_saveData.ComboDat);
        SpawnManager spawnManager = GameManager.FindFirstObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.Load(_saveData.EnemyData);
        }
        GameManager.Instance.cardman.Load(_saveData.CardDat);
    }
}
