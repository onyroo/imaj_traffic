using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputProperties : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    // Events عمومی برای هر Action
    public event Action OnWestPressed;
    public event Action OnNorthPressed;
    public event Action<Vector2> OnMoveInput;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        // if (playerInput != null)
        // {
           
        //     playerInput.actions["West"].performed += OnWest;
        //     playerInput.actions["North"].performed += OnNorth;
        //     playerInput.actions["Move"].performed += OnMove;
        //     playerInput.actions["Move"].canceled += OnMove; 
        // }
    }

    // private void OnDisable()
    // {
    //     if (playerInput != null)
    //     {
    //         playerInput.actions["West"].performed -= OnWest;
    //         playerInput.actions["North"].performed -= OnNorth;
    //         playerInput.actions["Move"].performed -= OnMove;
    //         playerInput.actions["Move"].canceled -= OnMove;
    //     }
    // }

    // Callback برای PlayerInput → event عمومی فراخوانی می‌شود
    public void OnWest(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnWestPressed?.Invoke();
        }
    }

    public void OnNorth(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnNorthPressed?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.ReadValue<Vector2>();
        OnMoveInput?.Invoke(move);
    }
}
