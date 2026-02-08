using UnityEngine;

public class CarMove : MonoBehaviour
{
    [SerializeField] private Vector2 limitMoveSpeed;
    [SerializeField] private Transform tr1, tr2;
    [SerializeField] private float wheelRadius = 0.35f;

    float moveSpeed;
    float targetSpeed;
    int c,v;

    void OnEnable()
    {
        moveSpeed = Random.Range(limitMoveSpeed.x, limitMoveSpeed.y);
        targetSpeed = moveSpeed;
    }

    void Update()
    {
        if(v>0&&c>0)
        {
        moveSpeed = Mathf.Lerp(moveSpeed, 0, Time.deltaTime * 15f);
        }
        else if(c>0||v>0)
        {
        moveSpeed = Mathf.Lerp(moveSpeed, 3, Time.deltaTime * 6f);
        }
        else 
        {
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * 0.5f);
        }

        Vector3 deltaMove = moveSpeed * transform.right * Time.deltaTime;
        transform.position += deltaMove;

        RotateWheels(deltaMove.magnitude);
    }

    void RotateWheels(float distance)
    {
        float rotationAngle = (distance / (2f * Mathf.PI * wheelRadius)) * 360f;

        tr1.Rotate(rotationAngle, 0f, 0f, Space.Self);
        tr2.Rotate(rotationAngle, 0f, 0f, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("car")||other.CompareTag("SafeWay"))
        {
            // targetSpeed = 3f;
            c++;
        }
        if(other.CompareTag("car")||other.CompareTag("Player")
        ||other.CompareTag("grandma"))
        {
            v++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("car")||other.CompareTag("SafeWay"))
        {
            c--;
            if (c <= 0)
            {
                c = 0;
                targetSpeed = Random.Range(limitMoveSpeed.x, limitMoveSpeed.y);
            }
        }
         
        if(other.CompareTag("car")||other.CompareTag("Player")
        ||other.CompareTag("grandma"))
        {
            v--;
        }
    }
}
