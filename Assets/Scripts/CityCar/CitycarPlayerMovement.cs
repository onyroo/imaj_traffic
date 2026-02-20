using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CitycarPlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private NavMeshAgent agent;
    private LineRenderer line;

    private Vector2 moveInput;

    [SerializeField] private int playerId;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float decelartion = 25f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float cooldown=2f;
    [SerializeField] private float speedSafe = 4;
    [Header("Car Control")]
    [SerializeField] private float sideFriction = 8f;
     
    [Header("Post System")]
    [SerializeField] private GameObject tr;
    [SerializeField] private Transform[] points;
    [SerializeField] private GameObject[] callers;

    bool havePost = false,collisionCar;
    GameObject g;

    float gasInput = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;

        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        if (line != null)
            line.enabled = false;

        StartCoroutine(SpawnerLoop());
    }

    private void OnEnable()
    {
        PlayerInputProperties p = PlayerJoinManager.Instance.playerInputSet(playerId);
        p.OnMoveInput += OnMove;
        p.OnWestPressed += takeNPC;
        p.OnRTValueChanged += OnGas;
    }

    private void OnDisable()
    {
        PlayerInputProperties p = PlayerJoinManager.Instance.playerInputSet(playerId);
        p.OnMoveInput -= OnMove;
        p.OnWestPressed -= takeNPC;
        p.OnRTValueChanged -= OnGas;
    }

    void OnGas(float a)
    {
        gasInput = Mathf.Clamp01(a);
        // Debug.Log(gasInput);
    }

    public void OnMove(Vector2 s)
    {
        moveInput = s.normalized;

        if (moveInput.sqrMagnitude > 0.01f||collisionCar)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void FixedUpdate()
    {
        RotateToMovement();

        Vector3 velocity = rb.linearVelocity;

        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        Vector3 sideVelocity = Vector3.Project(velocity, transform.right);

        rb.AddForce(-sideVelocity * sideFriction, ForceMode.Acceleration);

        if (gasInput > 0.01f&&!collisionCar)
        {
            if (forwardSpeed < maxSpeed)
            {
                rb.AddForce(transform.forward * acceleration * gasInput, ForceMode.Acceleration);
            }
        }
        else
        {
            if (Mathf.Abs(forwardSpeed) > 0.1f)
            {
                rb.AddForce(-transform.forward * forwardSpeed * decelartion, ForceMode.Acceleration);
            }
            else
            {
                rb.linearVelocity = new Vector3(0, velocity.y, 0);
            }
        }

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, rb.linearVelocity.y, flatVel.z);
        }
    }

    void RotateToMovement()
    {
        if (moveInput.sqrMagnitude < 0.01f) return;

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
    }

    void Update()
    {
        agent.nextPosition = transform.position;
        UpdatePathLine();
        mag=rb.linearVelocity.magnitude;
    }
float mag;
    IEnumerator SpawnerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));

            if (callers == null || callers.Length == 0)
                continue;

            int index = Random.Range(0, callers.Length);

            if (callers[index] != null && !callers[index].activeSelf)
                callers[index].SetActive(true);
        }
    }
    

    void UpdatePathLine()
    {
        if (!havePost || tr == null || line == null || !tr.activeSelf)
        {
            line.enabled = false;
            return;
        }

        NavMeshHit hit;
        Vector3 targetPos = tr.transform.position;
        if (NavMesh.SamplePosition(tr.transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            targetPos = hit.position;
        }

        NavMeshPath path = new NavMeshPath();
        bool found = NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path);

        if (!found || path.corners.Length < 2)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;
        line.positionCount = path.corners.Length;

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i] + Vector3.up * 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("SetDir") && !havePost)
        {
            g = other.gameObject;
            return;
        }

     
        if (other.CompareTag("Finish") && havePost)
        {
            havePost = false;

            if (tr != null)
                tr.SetActive(false);

            if (line != null)
                line.enabled = false;

            Debug.Log("Delivered");
            return;
        }
        Debug.Log(mag);
        if(mag < speedSafe||other.CompareTag("Finish"))return;
       
        collisionCar = true;

        // if (c != null)
        //     StopCoroutine(c);
        rb.linearVelocity=Vector3.zero;
        transform.position=new Vector3(0,transform.position.y,playerId);
        // c = StartCoroutine(colCoolDown(cooldown));
        Invoke("coold",cooldown);
        gameObject.SetActive(false);
    }
    void coold()
    {
        collisionCar = false;  
        gameObject.SetActive(true);
        // c = null;
    }
    
    // Coroutine c;
    // IEnumerator colCoolDown(float cl)
    // {
    //     yield return new WaitForSeconds(cl);
    //     collisionCar = false;  
    //     gameObject.SetActive(true);
    //     c = null;
    // }

    void takeNPC()
    {
        if (g == null || tr == null || points == null || points.Length == 0) return;

        havePost = true;
        g.SetActive(false);
        tr.SetActive(true);

        int randomIndex = Random.Range(0, points.Length);
        if (points[randomIndex] != null)
            tr.transform.position = points[randomIndex].position;

        g = null;
    }
}
