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
	public void OnTest(InputAction.CallbackContext ctx)
	{
	if(!ctx.performed) return;
	Debug.Log("Clicked : " + playerId.ToString());
	}
	//Cross Game Input
	public void OnRight(InputAction.CallbackContext ctx)
	{
	if(!ctx.performed) return;
	IntersectionGameManager.Instance.OnClicked(playerId,1);
	// Debug.Log("Clicked : " + playerId.ToString());
	}
	public void OnUp(InputAction.CallbackContext ctx)
	{
	if(!ctx.performed) return;
	IntersectionGameManager.Instance.OnClicked(playerId,2);
	// Debug.Log("Clicked : " + playerId.ToString());
	}
	public void OnLeft(InputAction.CallbackContext ctx)
	{
	if(!ctx.performed) return;
	IntersectionGameManager.Instance.OnClicked(playerId,3);
	// Debug.Log("Clicked : " + playerId.ToString());

	}
}
