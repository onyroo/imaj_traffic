using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerJoinManager : MonoBehaviour
{
    public static PlayerJoinManager Instance { get; private set; }

    private PlayerInputManager pim;

    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private Image bg;

    public List<int> players = new();
    private List<PlayerInputProperties> gamepads = new();
    private List<Gamepad> pads = new(); 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        pim = GetComponent<PlayerInputManager>();
    }

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(ChangeSceneAnim(sceneIndex));
    }

    private IEnumerator ChangeSceneAnim(int sceneIndex)
    {
        bg.gameObject.SetActive(true);
        Color c = bg.color;
        c.a = 0f;
        bg.color = c;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime * 5;
            bg.color = c;
            yield return null;
        }
        c.a = 1f;
        bg.color = c;

        yield return SceneManager.LoadSceneAsync(sceneIndex);

        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * 5;
            bg.color = c;
            yield return null;
        }
        c.a = 0f;
        bg.color = c;

        bg.gameObject.SetActive(false);
    }

    public void setSides(int player1,int player2)
    {
        players.Add(player1);
        players.Add(player2);
    } 
     
    public void OnPlayerJoined(PlayerInput player)
    {
        if (players.Count >= maxPlayers)
        {
            Destroy(player.gameObject);
            return;
        }

        Gamepad pad = player.devices.Count > 0 
            ? player.devices[0] as Gamepad 
            : null;

        if (pad == null)
        {
            Destroy(player.gameObject);
            return;
        }
 
        if (gamepads.Contains(player.GetComponent<PlayerInputProperties>()) || pads.Contains(pad))
        {
            Destroy(player.gameObject);
            return;
        }

        PlayerInputProperties props = player.GetComponent<PlayerInputProperties>();
        gamepads.Add(props);
        pads.Add(pad);

        int index = gamepads.Count - 1;
        if(gamepads.Count>=1)
            MenuManager.Instance.playersJoined(gamepads.Count);

        // if(gamepads.Count>1)
        //     ChangeScene(23);
        Debug.Log($"Player {index} Joined with {pad.displayName}");

        if (gamepads.Count >= maxPlayers)
        {
            pim.DisableJoining();
            Debug.Log("Joining Closed");
        }
    }

    public PlayerInputProperties playerInputSet(int playerId)
    {
        return gamepads[players[playerId]];
    }

    public Gamepad GetGamepad(int playerId)
    {
        if (playerId < 0 || playerId >= pads.Count) return null;
        return pads[players[playerId]];
    }

    public int playerCount()
    {
        return players.Count;
    }

    public int GetIndexGamepad(Gamepad playerGamepad)
    {
        return pads.IndexOf(playerGamepad);
    }

    public int PlayerCount => players.Count;
}
