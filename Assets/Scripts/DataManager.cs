using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public const int MaxHighScores = 10;
    private const string SaveFileName = "/savefile.json";
    private const string PlayerPrefsSaveKey = "SaveData";

    public string playerName;
    public string highScorePlayerName;
    public int highScore;
    public List<HighScoreEntry> highScores = new List<HighScoreEntry>();

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SyncWebGLFileSystem();
#endif

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
    public class HighScoreEntry
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public string highScorePlayerName;
        public int highScore;
        public HighScoreEntry[] highScores;
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
        if (score <= 0)
        {
            return;
        }

        string name = string.IsNullOrEmpty(playerName) ? "Unknown" : playerName;
        HighScoreEntry existing = FindEntryByName(name);

        if (existing != null)
        {
            if (score <= existing.score)
            {
                return;
            }

            existing.score = score;
            existing.playerName = name;
        }
        else
        {
            highScores.Add(new HighScoreEntry
            {
                playerName = name,
                score = score
            });
        }

        SortAndTrimHighScores();
        SyncBestScore();
        Save();
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public string GetHighScorePlayerName()
    {
        return highScorePlayerName;
    }

    public string GetHighScoresDisplay()
    {
        if (highScores == null || highScores.Count == 0)
        {
            return "No high scores yet.";
        }

        var lines = new List<string>();
        for (int i = 0; i < highScores.Count; i++)
        {
            string name = string.IsNullOrEmpty(highScores[i].playerName)
                ? "Unknown"
                : highScores[i].playerName;
            lines.Add($"{i + 1}. {name} : {highScores[i].score}");
        }

        return string.Join("\n", lines);
    }

    public void Save()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.highScorePlayerName = highScorePlayerName;
        data.highScore = highScore;
        data.highScores = highScores.ToArray();

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + SaveFileName, json);
        PersistForWebGL(json);
    }

    public void Load()
    {
        string json = ReadSavedJson();
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
        {
            return;
        }

        playerName = data.playerName;
        highScorePlayerName = data.highScorePlayerName;
        highScore = data.highScore;

        if (data.highScores != null && data.highScores.Length > 0)
        {
            highScores = new List<HighScoreEntry>(data.highScores);
        }
        else if (highScore > 0)
        {
            highScores = new List<HighScoreEntry>
            {
                new HighScoreEntry
                {
                    playerName = highScorePlayerName,
                    score = highScore
                }
            };
        }

        SortAndTrimHighScores();
        SyncBestScore();
        Save();
    }

    private static string ReadSavedJson()
    {
        string path = Application.persistentDataPath + SaveFileName;
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        if (PlayerPrefs.HasKey(PlayerPrefsSaveKey))
        {
            return PlayerPrefs.GetString(PlayerPrefsSaveKey);
        }
#endif
        return null;
    }

    private static void PersistForWebGL(string json)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerPrefs.SetString(PlayerPrefsSaveKey, json);
        PlayerPrefs.Save();
        SyncWebGLFileSystem();
#endif
    }

    private HighScoreEntry FindEntryByName(string name)
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            if (string.Equals(highScores[i].playerName, name, System.StringComparison.OrdinalIgnoreCase))
            {
                return highScores[i];
            }
        }

        return null;
    }

    private void KeepBestScorePerPlayer()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            if (string.IsNullOrEmpty(highScores[i].playerName))
            {
                highScores[i].playerName = "Unknown";
            }

            for (int j = highScores.Count - 1; j > i; j--)
            {
                if (!string.Equals(highScores[i].playerName, highScores[j].playerName, System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (highScores[j].score > highScores[i].score)
                {
                    highScores[i].score = highScores[j].score;
                    highScores[i].playerName = highScores[j].playerName;
                }

                highScores.RemoveAt(j);
            }
        }
    }

    private void SortAndTrimHighScores()
    {
        KeepBestScorePerPlayer();
        highScores.Sort((a, b) => b.score.CompareTo(a.score));

        if (highScores.Count > MaxHighScores)
        {
            highScores.RemoveRange(MaxHighScores, highScores.Count - MaxHighScores);
        }
    }

    private void SyncBestScore()
    {
        if (highScores.Count == 0)
        {
            highScore = 0;
            highScorePlayerName = string.Empty;
            return;
        }

        highScore = highScores[0].score;
        highScorePlayerName = highScores[0].playerName;
    }
}
