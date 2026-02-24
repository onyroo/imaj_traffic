using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
public class ShowScorePanel : MonoBehaviour
{
    [SerializeField] private GameObject thisPanel;
    [SerializeField] private RTLTextMeshPro playerTotalText1,playerTotalText2;
    [SerializeField] private RTLTextMeshPro playerText1,playerText2;
    [SerializeField] private RTLTextMeshPro playerRank1,playerRank2;

    public void PopUp(int PlayerScore1,int PlayerScore2)
    {


 

        playerText1.text=PlayerScore1.ToString();
        playerText2.text=PlayerScore2.ToString();
        PlayerPrefs.SetInt("player1Score", PlayerPrefs.GetInt("player1Score", 0)+PlayerScore1);
        PlayerPrefs.SetInt("player2Score", PlayerPrefs.GetInt("player2Score", 0)+PlayerScore2);
        PlayerPrefs.Save();

        playerTotalText1.text=PlayerPrefs.GetInt("player1Score", 0).ToString();
        playerTotalText2.text=PlayerPrefs.GetInt("player2Score", 0).ToString();
            // PlayerPrefs.SetString("player1",playerName1);
            // PlayerPrefs.SetString("player2",playerName2);
            // PlayerPrefs.SetInt("player1Score", 0);
            // PlayerPrefs.SetInt("player2Score", 0);
            // PlayerPrefs.Save();
        CarGameDatabase database = new CarGameDatabase();
        database.UpdateScoreInfo(PlayerPrefs.GetString("player1"),(int)PlayerPrefs.GetInt("player1Score", 0));
        database.UpdateScoreInfo(PlayerPrefs.GetString("player2"),(int)PlayerPrefs.GetInt("player2Score", 0));
        playerRank1.text=database.GetRankByName(PlayerPrefs.GetString("player1")).ToString(); 
        playerRank2.text=database.GetRankByName(PlayerPrefs.GetString("player2")).ToString();
        thisPanel.SetActive(true);
        
    }

    public void ChangeScene(int a)
    {
        PlayerJoinManager.Instance.ChangeScene(a);
    }
}
