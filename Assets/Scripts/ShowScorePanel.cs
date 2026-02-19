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
        PlayerPrefs.SetFloat("player1Score", PlayerPrefs.GetFloat("player1Score", 0f)+PlayerScore1);
        PlayerPrefs.SetFloat("player2Score", PlayerPrefs.GetFloat("player2Score", 0f)+PlayerScore2);
        PlayerPrefs.Save();

        playerTotalText1.text=PlayerPrefs.GetFloat("player1Score", 0f).ToString();
        playerTotalText2.text=PlayerPrefs.GetFloat("player2Score", 0f).ToString();
            // PlayerPrefs.SetString("player1",playerName1);
            // PlayerPrefs.SetString("player2",playerName2);
            // PlayerPrefs.SetFloat("player1Score", 0f);
            // PlayerPrefs.SetFloat("player2Score", 0f);
            // PlayerPrefs.Save();
        CarGameDatabase database = new CarGameDatabase();
        database.UpdateScoreInfo(PlayerPrefs.GetString("player1"),(int)PlayerPrefs.GetFloat("player1Score", 0f));
        database.UpdateScoreInfo(PlayerPrefs.GetString("player2"),(int)PlayerPrefs.GetFloat("player2Score", 0f));
        playerRank1.text=database.GetRankByName(PlayerPrefs.GetString("player1")).ToString(); 
        playerRank2.text=database.GetRankByName(PlayerPrefs.GetString("player2")).ToString();
        thisPanel.SetActive(true);
        
    }

    public void ChangeScene(int a)
    {
        PlayerJoinManager.Instance.ChangeScene(a);
    }
}
