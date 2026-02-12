using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrandmaProperty : MonoBehaviour
{
    public int side = 0;
    public int lastSide = 0;
    public bool canMove = true;

    private Rigidbody rb;

    [SerializeField] float moveSpeed = 1.8f;
    [SerializeField] float turnSmooth = 4f;
    [SerializeField] float rayDistance = 1.5f;
    [SerializeField] float edgeAvoidStrength = 2.5f;
    [SerializeField] float reachCenterDistance = 0.6f;
    [SerializeField] private Animator anim;
    [SerializeField] GameObject dis1,dis2;
    Vector3 moveDir;
    bool forceReturnToCenter;

    Transform sideCenter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveDir = transform.forward;
    }

    private void Start()
    {
        
        anim.SetInteger("walk",1);
        lastSide=side;
        setPlayerParent();
    }
    public Animator animReturn()
    {
       return anim; 
    }
 
    public void setPlayerParent()
    {
 
        if(lastSide == 0)
        {
           sideCenter= GrandmaGameManager.Instance.sideSpawnPointA;
           dis1.SetActive(false);
           dis2.SetActive(true);
        }
        else
        {
           sideCenter= GrandmaGameManager.Instance.sideSpawnPointB;
            dis1.SetActive(true);
           dis2.SetActive(false);
        }
         
    }
     private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("side"))
        {
            // if(transform.parent!=null)
            //     lastSide=(other.name=="0")?0:1;
            if(other.name!=side.ToString())
            {
                 
                //  setPlayerParent();
                //  GrandmaGameManager.Instance.ResetGrandmaPosition(transform,side);
            }
        }
        else if(other.CompareTag("car"))
        {
            if(transform.parent!=null)
            {
                transform.parent.gameObject.GetComponent<PlayerMovementRoadGame>().grandmaTakeDown();
            }
            GrandmaGameManager.Instance.ResetGrandmaPosition(transform,lastSide,side);
            // setPlayerParent();
        }
        else if(other.CompareTag("Finish"))
        {
            Destroy(gameObject,2);
        }
        
    } 
    public void goToDie()
    {
        if(lastSide==0)
            sideCenter= GrandmaGameManager.Instance.diePoint1;
        else 
            sideCenter= GrandmaGameManager.Instance.diePoint2;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Respawn"))
            forceReturnToCenter = true;
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        if (forceReturnToCenter)
        {
            ReturnToCenter();
            return;
        }

        HumanLikeMove();
    }

    void HumanLikeMove()
    {
        Vector3 centerDir = (sideCenter.position - transform.position).normalized;
        float distFromCenter = Vector3.Distance(transform.position, sideCenter.position);

        Vector3 desiredDir = moveDir;

        if (Physics.Raycast(transform.position, transform.forward, rayDistance))
            desiredDir = Quaternion.Euler(0, Random.Range(-100f, 100f), 0) * moveDir;

        if (distFromCenter > 2.2f)
            desiredDir = Vector3.Lerp(desiredDir, centerDir, edgeAvoidStrength * Time.fixedDeltaTime);

        moveDir = Vector3.Slerp(moveDir, desiredDir.normalized, turnSmooth * Time.fixedDeltaTime);

        Move(moveDir);
    }

    void ReturnToCenter()
    {
        Vector3 dir = (sideCenter.position - transform.position).normalized;
        moveDir = Vector3.Slerp(moveDir, dir, turnSmooth * Time.fixedDeltaTime);

        Move(moveDir);

        if (Vector3.Distance(transform.position, sideCenter.position) < reachCenterDistance)
        {
            forceReturnToCenter = false;
            RandomizeDir();
        }
    }

    void Move(Vector3 dir)
    {
        Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up);

        if (flatDir.sqrMagnitude < 0.001f)
            return;

        Quaternion rot = Quaternion.LookRotation(flatDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSmooth * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + flatDir.normalized * moveSpeed * Time.fixedDeltaTime);
    }


    void RandomizeDir()
    {
        moveDir = Quaternion.Euler(0, Random.Range(-140f, 140f), 0) * Vector3.forward;
        moveDir = transform.rotation * moveDir.normalized;
    }
}
