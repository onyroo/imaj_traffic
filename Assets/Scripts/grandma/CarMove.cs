using UnityEngine;

public class CarMove : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private Vector2 limitMoveSpeed = new Vector2(6f, 9f);
    [SerializeField] private float stopSpeed = 0f;
    [SerializeField] private float slowSpeed = 3f;

    [Header("Wheels")]
    [SerializeField] private Transform tr1, tr2;
    [SerializeField] private float wheelRadius = 0.35f;

    [Header("Smooth")]
    [SerializeField] private float brakeSmooth = 8f;
    [SerializeField] private float accelSmooth = 2f;
    [SerializeField] private float reducedAccelSmooth = 0.6f;

    [Header("Recovery")]
    [SerializeField] private float accelRecoverThreshold = 3f;

    float moveSpeed;
    float targetSpeed;

    int carsAhead;
    int humansAhead;
    int safeWayCount;

    // bool lowAccelMode;           
    bool lowAccelDueToCar;        

    void OnEnable()
    {
        targetSpeed = Random.Range(limitMoveSpeed.x, limitMoveSpeed.y);
        moveSpeed = targetSpeed;
        // lowAccelMode = false;
        lowAccelDueToCar = false;
    }

    void Update()
    {
        UpdateTargetSpeed();

        if (carsAhead > 0)
        {
            // lowAccelMode = true;
            lowAccelDueToCar = true;
        }

        if (carsAhead == 0 && moveSpeed >= accelRecoverThreshold)
        {
            lowAccelDueToCar = false;
        }

        float accel = lowAccelDueToCar ? reducedAccelSmooth : accelSmooth;
        float smooth = moveSpeed > targetSpeed ? brakeSmooth : accel;

        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * smooth);

        Vector3 deltaMove = transform.right * moveSpeed * Time.deltaTime;
        transform.position += deltaMove;

        RotateWheels(deltaMove.magnitude);
    }

    void UpdateTargetSpeed()
    {
        if (carsAhead > 0)
        {
            targetSpeed = stopSpeed;
            return;
        }

        if (safeWayCount > 0 && humansAhead > 0)
        {
            targetSpeed = stopSpeed;
            return;
        }

        if (safeWayCount > 0)
        {
            targetSpeed = slowSpeed;
            return;
        }

        targetSpeed = Mathf.Clamp(targetSpeed, limitMoveSpeed.x, limitMoveSpeed.y);

        if (targetSpeed < limitMoveSpeed.x)
            targetSpeed = Random.Range(limitMoveSpeed.x, limitMoveSpeed.y);
    }

    void RotateWheels(float distance)
    {
        float angle = (distance / (2f * Mathf.PI * wheelRadius)) * 360f;
        if (tr1) tr1.Rotate(angle, 0f, 0f, Space.Self);
        if (tr2) tr2.Rotate(angle, 0f, 0f, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("car")) carsAhead++;
        if (other.CompareTag("Player"))
        {
        if(other.GetComponent<PlayerMovementRoadGame>().SafeWay)
            humansAhead++;    
        } 
        if (other.CompareTag("grandma"))
        {
        if(other.GetComponent<GrandmaProperty>().SafeWay)
            humansAhead++;    
        } 
        if (other.CompareTag("SafeWay")) safeWayCount++;
        if (other.CompareTag("destroy")) Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("car")) carsAhead = Mathf.Max(0, carsAhead - 1);
        else if (other.CompareTag("Player") || other.CompareTag("grandma")) humansAhead = Mathf.Max(0, humansAhead - 1);
        else if (other.CompareTag("SafeWay")) safeWayCount = Mathf.Max(0, safeWayCount - 1);
    }
}
