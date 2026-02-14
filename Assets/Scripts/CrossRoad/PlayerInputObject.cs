using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputObject : MonoBehaviour
{
     
    [SerializeField] private int playerId;
 
 
	  private void OnEnable() 
    {
        PlayerInputProperties p = PlayerJoinManager.Instance.playerInputSet(playerId);

        p.OnMoveInput += OnMove;          
 
    }
    private void OnDisable() {
        PlayerInputProperties p = PlayerJoinManager.Instance.playerInputSet(playerId);

        p.OnMoveInput -= OnMove;          
 
    }
	bool a;
	public void OnMove(Vector2 rawInput)
	{
	 
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
