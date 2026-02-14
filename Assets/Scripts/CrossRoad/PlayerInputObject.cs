using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputObject : MonoBehaviour
{
     
    int playerId;
    [SerializeField] private PlayerInput input;
	void Awake()
	{
	//input.GetComponent<PlayerInput>();
	playerId=input.playerIndex;
	Debug.Log("Im player And my id : " + playerId.ToString());
	}
	// public void OnTest(InputAction.CallbackContext ctx)
	// {
	// if(!ctx.performed) return;
	// Debug.Log("Clicked : " + playerId.ToString());
	// }
	// //Cross Game Input
	// public void OnRight(InputAction.CallbackContext ctx)
	// {
	// if(!ctx.performed) return;
	// CrossRoadGameManager.Instance.OnClicked(playerId,1);
	// // Debug.Log("Clicked : " + playerId.ToString());
	// }
	// public void OnUp(InputAction.CallbackContext ctx)
	// {
	// if(!ctx.performed) return;
	// CrossRoadGameManager.Instance.OnClicked(playerId,2);
	// // Debug.Log("Clicked : " + playerId.ToString());
	// }
	// public void OnLeft(InputAction.CallbackContext ctx)
	// {
	// if(!ctx.performed) return;
	// CrossRoadGameManager.Instance.OnClicked(playerId,3);
	// // Debug.Log("Clicked : " + playerId.ToString());

	// }
	// public void OnDown(InputAction.CallbackContext ctx)
	// {
	// if(!ctx.performed) return;
	// CrossRoadGameManager.Instance.OnClicked(playerId,0);
	// // Debug.Log("Clicked : " + playerId.ToString());

	// }
bool a;
public void OnMove(InputAction.CallbackContext ctx)
{
    Vector2 rawInput = ctx.ReadValue<Vector2>();
    if (rawInput.sqrMagnitude < 0.01f) {a=true;return;}
	if(!a)return;
	a=false;
    if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
    {
        if (rawInput.x > 0)
            CrossRoadGameManager.Instance.OnClicked(playerId, 1); // Right
        else
            CrossRoadGameManager.Instance.OnClicked(playerId, 3); // Left
    }
    else
    {
        if (rawInput.y > 0)
            CrossRoadGameManager.Instance.OnClicked(playerId, 2); // Up
        else
            CrossRoadGameManager.Instance.OnClicked(playerId, 0); // Down
    }
}


}
