using UnityEngine;

public class CarMove : MonoBehaviour
{
    [SerializeField] private Vector2 limitMoveSpeed;
    [SerializeField] private Transform tr1, tr2;
    [SerializeField] private float wheelRadius = 0.35f;

    float moveSpeed;
    float targetSpeed;
    int c;

    void OnEnable()
    {
        moveSpeed = Random.Range(limitMoveSpeed.x, limitMoveSpeed.y);
        targetSpeed = moveSpeed;
    }

    void Update()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * 6f);

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
        if (other.CompareTag("SafeWay") || other.CompareTag("car"))
        {
            targetSpeed = 3f;
            c++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeWay") || other.CompareTag("car"))
        {
            c--;
            if (c <= 0)
            {
                c = 0;
                targetSpeed = 12f;
            }
        }
    }
}
