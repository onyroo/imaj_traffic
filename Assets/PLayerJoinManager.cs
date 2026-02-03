using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    private int players = 0;
    private PlayerInputManager pim;

    private void Awake()
    {
        pim = GetComponent<PlayerInputManager>();
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        players++;

        if (players >= 2)
        {
            pim.DisableJoining();
        }
    }
}
