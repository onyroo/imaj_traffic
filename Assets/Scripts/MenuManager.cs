using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using UnityEngine.InputSystem;
using System.Collections;
// using UnityEngine.EventSystems;
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] private int levelCount=2;
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private GameObject playerSide1,playerSide2;
    [SerializeField] private GameObject playerIconName1,playerIconName2;

    [SerializeField] private GameObject PLayPanel;
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private GameObject UserNamePanel;
    [SerializeField] private Slider slider1, slider2;
    [SerializeField] private RTLTextMeshPro nameText;
    [SerializeField] private GameObject textHelper;
    [SerializeField] private AudioSource clickSound,hoverSound;
    // [SerializeField] private Button playButton;
    private string playerName1,playerName2; 
    CarGameDatabase database;
    
    void Awake()
    {
  
        Instance = this;
        database = new CarGameDatabase();
        // playButton.Press;
    }
    
    private void Start()
    {
     
        playersJoined(PlayerJoinManager.Instance.playerCount());
        for (int i = 0; i < levelCount; i++)
        {
        PlayerPrefs.SetInt("level" + i.ToString(), 0);
        PlayerPrefs.Save();
            
        }
        slider1.value = 1;
        slider2.value = 0;
    }
    public void _playClickSound()
    {
        clickSound.Play();
    }

    public void _playHoverSound()
    {
        hoverSound.Play();
    }
    void Update()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            var pad = Gamepad.all[i];

            if (pad.buttonSouth.wasPressedThisFrame)
            {
                // Debug.Log($"Button A pressed by Player {i} ({pad.displayName})");
                _playClickSound();
            }

            if (pad.buttonEast.wasPressedThisFrame)
            {
                // Debug.Log($"Button B pressed by Player {i} ({pad.displayName})");
                if(scoreBoardPanel.activeSelf)
                {
                    scoreBoardPanel.SetActive(false);
                    
                    _playClickSound();
                    PLayPanel.SetActive(true);
                }
            }

           
            float stickX = pad.leftStick.x.ReadValue();

 
            bool stickRight = stickX > 0.5f;
            bool stickLeft = stickX < -0.5f;

            if (pad.dpad.right.wasPressedThisFrame || pad.dpad.left.wasPressedThisFrame || stickRight || stickLeft)
            {
                int playerId = PlayerJoinManager.Instance.GetIndexGamepad(pad);

                int direction = (pad.dpad.right.wasPressedThisFrame || stickRight) ? 1 : -1;

                if (playerId == 0 && playerSide1.activeSelf)
                {
                    StartCoroutine(ChangeSlider(playerId, direction));
                }
                else if (playerId == 1 && playerSide2.activeSelf)
                {
                    StartCoroutine(ChangeSlider(playerId, direction));
                }
            }


        }
    }

    public void playersJoined(int a)
    {
        if(a==1)
        {
        
            playerSide1.SetActive(true);
        }
        else if(a>1)
        {
        
            playerSide2.SetActive(true);
        // EventSystem.current.SetSelectedGameObject(playButton);
        }
    }
    // void turnOnPlayPanel()
    // {
    //     PLayPanel.SetActive(true);
    // }
    int nextPanelText;
    public void SetName(string s)
    {
        
        if(nextPanelText==0)
        {
            playerName1+=s;
            Debug.Log(playerName1);
            nameText.text=playerName1;
            nameText.isRightToLeftText = true;
            textHelper.SetActive(false);
        }
        else
        {
            playerName2+=s;
            nameText.text=playerName2;
            nameText.isRightToLeftText = true;
            textHelper.SetActive(false);
        }
    }
    public void RemoveName()
    {
        
        if (nextPanelText == 0)
        {
            if (playerName1.Length > 0)
            {
                playerName1 = playerName1.Substring(0, playerName1.Length - 1);
 
                nameText.text = playerName1;
                textHelper.SetActive(false);
                if (playerName1.Length == 0)
                {
                    textHelper.SetActive(true);
                }
            }
            else
            {
                textHelper.SetActive(true);
            }
        }
        else
        {
            if (playerName2.Length > 0)
            {
                playerName2 = playerName2.Substring(0, playerName2.Length - 1);
                nameText.text = playerName2;
                textHelper.SetActive(false);
                if (playerName2.Length == 0)
                {
                    textHelper.SetActive(true);
                }
            }
            else
            {
                textHelper.SetActive(true);
            }
        }
    }

    public void SetColor(int c)
    {
        if(nextPanelText==0)
        {
            
        }
        else
        {
            
        }
    }
    public void nextPlayerOnSetName()
    {
        if(nextPanelText==0&&playerName1.Length>=3&&!database.HasPlayer(playerName1))
        {
        PlayerJoinManager.Instance.SetGamePadForUI(1);

            nextPanelText++;
            nameText.text="";
            database.SetPlayerInfo(playerName1,0);
            textHelper.SetActive(true);
            playerIconName1.SetActive(false);
            playerIconName2.SetActive(true);
        }
        else if(playerName2.Length>=3&&!database.HasPlayer(playerName2))
        {
            UserNamePanel.SetActive(false);
            // choosePanel.SetActive(true);
            database.SetPlayerInfo(playerName2,0);
            PlayerPrefs.SetString("player1",playerName1);
            PlayerPrefs.SetString("player2",playerName2);
            PlayerPrefs.SetInt("player1Score", 0);
            PlayerPrefs.SetInt("player2Score", 0);
            PlayerPrefs.Save();
            PlayerJoinManager.Instance.SetGamePadForUI(0);

            PlayerJoinManager.Instance.ChangeScene(1);
        }
    }

    bool CanSetSide;
    public void setSides()
    {
        if((slider1.value== slider2.value)||(slider1.value<1&&slider1.value>0)&&(slider2.value<1&&slider2.value>0)
        ||!playerSide2.activeSelf)
        {
         
            
        }
        else
        {

            if (!CanSetSide)
            {
            CanSetSide=true;
            return;
                
            }
            if(slider1.value==1)
                PlayerJoinManager.Instance.setSides(0,1);
            else
                PlayerJoinManager.Instance.setSides(1,0);
        sidePanel.SetActive(false);
        PLayPanel.SetActive(true);

        PlayerJoinManager.Instance.SetGamePadForUI(0);
        }
    }
    IEnumerator ChangeSlider(int playerId, int direction)
    {
        Slider s = (playerId == 0) ? slider1 : slider2;
        if((s.value==1&&(direction < 0))||(s.value==0&&(direction > 0))&&sidePanel.activeSelf)
        {
        _playHoverSound();
        float target = (direction > 0) ? 1f : 0f;
        float startValue = s.value;
        float t = 0f;

        while (t<0.98f&&t>=0)
        {
            t += Time.deltaTime * 4f;  
            s.value = Mathf.Lerp(startValue, target, t);
            yield return null;
        }

        s.value = target; 
        }
    }



}
