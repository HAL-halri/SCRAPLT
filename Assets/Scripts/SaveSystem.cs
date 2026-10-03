using UnityEngine;

public static class SaveSystem
{
    const string SAVE_KEY = "GameSaveData";

    public static void Save(GameManager gameManager)
    {
        SaveData data = gameManager.GetSaveData();
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}
