using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarGameMovement : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask obstacleMask;

    private int playerId;

    private void Awake()
    {
        playerId = input.playerIndex;
        Debug.Log("Player ID: " + playerId);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (ParkingGameManager.Instance.playerIdTurn != playerId) return;
        if (ParkingGameManager.Instance.moving) return;
        Vector2 rawInput = ctx.ReadValue<Vector2>();
        if (rawInput.sqrMagnitude < 0.01f) return;

        Vector2 dir2D;
        if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
            dir2D = rawInput.x > 0 ? Vector2.right : Vector2.left;
        else
            dir2D = rawInput.y > 0 ? Vector2.up : Vector2.down;

        Vector3 moveDir = new Vector3(dir2D.x, 0f, dir2D.y);
		Debug.Log(moveDir);
        transform.rotation = Quaternion.LookRotation(moveDir);

        RaycastHit hit;
        Vector3 target;

        if (Physics.Raycast(transform.position, moveDir, out hit, rayDistance, obstacleMask))
            {
				target = hit.point - moveDir * 0.5f;
				Debug.Log(hit.point);
			}
        else
		{
			target = transform.position + moveDir * rayDistance;
			Debug.Log(target);

		}
			

        ParkingGameManager.Instance.setMove();
        StartCoroutine(MoveCar(target));
    }

    IEnumerator MoveCar(Vector3 target)
    {
        Vector3 start = transform.position;
        float duration = Vector3.Distance(start, target) / moveSpeed;
		
		float t = 0f;
		while (t < 1f)
		{
		    t += Time.deltaTime / duration;
		    transform.position = Vector3.Lerp(start, target, t);
		    yield return null;
		}


        transform.position = target;
        ParkingGameManager.Instance.setTurn();
    }
}
