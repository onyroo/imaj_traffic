using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int score;

    public PlayerData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
    public int rank;

    public LeaderboardEntry(string name, int score, int rank)
    {
        this.name = name;
        this.score = score;
        this.rank = rank;
    }
}

[System.Serializable]
public class PlayerDatabaseWrapper
{
    public List<PlayerData> players = new List<PlayerData>();
}

[System.Serializable]
public class SerializationHelper<T>
{
    public List<T> list;

    public SerializationHelper(List<T> list)
    {
        this.list = list;
    }
}

public class CarGameDatabase
{
    private const string SAVE_KEY = "CAR_GAME_DATABASE";

    private PlayerDatabaseWrapper database = new PlayerDatabaseWrapper();

    public CarGameDatabase()
    {
        LoadFromDisk();
    }

    public void SetPlayerInfo(string name, int score)
    {
        PlayerData existing = database.players.Find(p => p.name == name);
        if (existing != null) return;

        database.players.Add(new PlayerData(name, score));
        SaveToDisk();
    }

    public void UpdateScoreInfo(string name, int score)
    {
        PlayerData player = database.players.Find(p => p.name == name);
        if (player == null) return;

        player.score = score;
        SaveToDisk();
    }

    public int GetRankByName(string name)
    {
        var sorted = database.players
            .OrderByDescending(p => p.score)
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].name == name)
                return i + 1;
        }

        return -1;
    }

    public List<LeaderboardEntry> LoadDatabase()
    {
        var sorted = database.players
            .OrderByDescending(p => p.score)
            .ToList();

        List<LeaderboardEntry> result = new List<LeaderboardEntry>();

        for (int i = 0; i < sorted.Count; i++)
        {
            result.Add(new LeaderboardEntry(
                sorted[i].name,
                sorted[i].score,
                i + 1
            ));
        }

        return result;
    }

    public string GetJsonLoadData()
    {
        List<LeaderboardEntry> leaderboard = LoadDatabase();
        return JsonUtility.ToJson(
            new SerializationHelper<LeaderboardEntry>(leaderboard),
            true
        );
    }
    public bool HasPlayer(string name)
    {
        return database.players.Exists(p => p.name == name);
    }
    private void SaveToDisk()
    {
        string json = JsonUtility.ToJson(database);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadFromDisk()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            database = new PlayerDatabaseWrapper();
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        database = JsonUtility.FromJson<PlayerDatabaseWrapper>(json);

        if (database == null)
            database = new PlayerDatabaseWrapper();
    }

    public void ClearDatabase()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        database = new PlayerDatabaseWrapper();
    }
}
