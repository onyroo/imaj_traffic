using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputProperties : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    public event Action OnWestPressed;
    public event Action OnNorthPressed;
    public event Action OnSouthReleased;
    public event Action OnSouthPressed;
    public event Action<Vector2> OnMoveInput;
    // public event Action OnRTHold;
    public event Action<float> OnRTValueChanged;  

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        // if (playerInput != null)
        // {
            // playerInput.actions["West"].performed += OnWest;
            // playerInput.actions["North"].performed += OnNorth;
            // playerInput.actions["Move"].performed += OnMove;
            // playerInput.actions["Move"].canceled += OnMove;
            // playerInput.actions["RT"].performed += OnRT;
            // playerInput.actions["RT"].canceled += OnRT;
        // }
    }

    // private void OnDisable()
    // {
        // if (playerInput != null)
        // {
            // playerInput.actions["West"].performed -= OnWest;
            // playerInput.actions["North"].performed -= OnNorth;
            // playerInput.actions["Move"].performed -= OnMove;
            // playerInput.actions["Move"].canceled -= OnMove;
            // playerInput.actions["RT"].performed -= OnRT;
            // playerInput.actions["RT"].canceled -= OnRT;
        // }
    // }

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

    public void OnSouth(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnSouthPressed?.Invoke();
        }
        else if(ctx.canceled)
        {
            OnSouthReleased?.Invoke();
        }
    }
 

    public void OnRT(InputAction.CallbackContext ctx)
    {
        // if(ctx.performed)
            OnRTValueChanged?.Invoke(ctx.ReadValue<float>());

        
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.ReadValue<Vector2>();
        OnMoveInput?.Invoke(move);
    }
}
