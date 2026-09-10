using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string playerName;
    public string highScorePlayerName;
    public int highScore;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }


    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public string highScorePlayerName;
        public int highScore;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        Save();
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void TryUpdateHighScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            highScorePlayerName = playerName;
            Save();
        }
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public string GetHighScorePlayerName()
    {
        return highScorePlayerName;
    }

    public void Save()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.highScorePlayerName = highScorePlayerName;
        data.highScore = highScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            playerName = data.playerName;
            highScorePlayerName = data.highScorePlayerName;
            highScore = data.highScore;
        }
    }
}
