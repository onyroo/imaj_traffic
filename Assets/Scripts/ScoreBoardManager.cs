using System.Collections.Generic;
using UnityEngine;
using RTLTMPro;

public class ScoreBoardManager : MonoBehaviour
{
    private List<Carts> LeaderBoardCarts = new List<Carts>();

    [SerializeField] private GameObject cartObj;
    [SerializeField] private Transform BoardArea;

    private List<LeaderboardEntry> leaderboardData;

    private void OnEnable()
    {
        CarGameDatabase database = new CarGameDatabase();
        string json = database.GetJsonLoadData();

        leaderboardData = JsonUtility.FromJson<SerializationHelper<LeaderboardEntry>>(json).list;
        GenerateLeaderBoard();
    }

    private void OnDisable()
    {
        foreach (Transform child in BoardArea)
        {
            Destroy(child.gameObject);
        }
        LeaderBoardCarts.Clear();
    }

    private int GetPlayerCount()
    {
        if (leaderboardData == null) return 0;
        return leaderboardData.Count;
    }

    private void GenerateLeaderBoard()
    {
        for (int i = 0; i < GetPlayerCount(); i++)
        {
            GameObject g = Instantiate(cartObj, BoardArea);
            Carts cartProperty = g.GetComponent<Carts>();

            LeaderboardEntry entry = leaderboardData[i];

            cartProperty.playerName.text = entry.name;
            cartProperty.playerRank.text = entry.rank.ToString();
            cartProperty.playerScore.text = entry.score.ToString();

            LeaderBoardCarts.Add(cartProperty);
        }
    }
}
