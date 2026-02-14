using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] private GameObject JoinPanel;
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private GameObject UserNamePanel;
    [SerializeField] private Slider slider1, slider2;
    [SerializeField] private Text nameText;
    private string playerName1,playerName2; 
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PlayerJoinManager.Instance.playerCount() > 1)
        {
            JoinPanel.SetActive(false);
        }

        slider1.value = 1;
        slider2.value = 0;
    }

    void Update()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            var pad = Gamepad.all[i];

            if (pad.buttonSouth.wasPressedThisFrame)
            {
                Debug.Log($"Button A pressed by Player {i} ({pad.displayName})");
            }

            if (pad.buttonEast.wasPressedThisFrame)
            {
                Debug.Log($"Button B pressed by Player {i} ({pad.displayName})");
            }

            // چک حرکت slider
            if (pad.dpad.right.wasPressedThisFrame || pad.dpad.left.wasPressedThisFrame)
            {
                int playerId = PlayerJoinManager.Instance.GetIndexGamepad(pad);
                if (playerId != -1)
                {
                    StartCoroutine(ChangeSlider(playerId, pad.dpad.right.wasPressedThisFrame ? 1 : -1));
                }
            }
        }
    }

    public void playersJoined()
    {
        JoinPanel.SetActive(false);
    }
    int nextPanelText;
    public void SetName(string s)
    {
        if(nextPanelText==0)
        {
            playerName1+=s;
            // Debug.Log(playerName1);
            nameText.text=playerName1;
        }
        else
        {
            playerName2+=s;
            nameText.text=playerName2;
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
            }
        }
        else
        {
            if (playerName2.Length > 0)
            {
                playerName2 = playerName2.Substring(0, playerName2.Length - 1);
                nameText.text = playerName2;
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
        if(nextPanelText==0&&playerName1!=null)
        {
            nextPanelText++;
            nameText.text="";
        }
        else if(playerName2!=null)
        {
            UserNamePanel.SetActive(false);
            // choosePanel.SetActive(true);
            PlayerPrefs.SetString("player1",playerName1);
            PlayerPrefs.SetString("player2",playerName2);
            PlayerPrefs.Save();
            PlayerJoinManager.Instance.ChangeScene(1);
        }
    }
    // direction: 1 → right /  -1 → left
    public void setSides()
    {
        if(slider1.value== slider2.value)
        {
         
            
        }
        else
        {
            if(slider1.value==1)
                PlayerJoinManager.Instance.setSides(0,1);
            else
                PlayerJoinManager.Instance.setSides(1,0);
        sidePanel.SetActive(false);
        UserNamePanel.SetActive(true);
        }
    }
    IEnumerator ChangeSlider(int playerId, int direction)
    {
        Slider s = (playerId == 0) ? slider1 : slider2;
        if(s.value==1||s.value==0)
        {
        float target = (direction > 0) ? 1f : 0f;
        float startValue = s.value;
        float t = 0f;

        while (!Mathf.Approximately(s.value, target))
        {
            t += Time.deltaTime * 4f;  
            s.value = Mathf.Lerp(startValue, target, t);
            yield return null;
        }

        s.value = target; 
        }
    }



}
