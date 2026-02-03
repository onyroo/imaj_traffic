using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRoadGame : MonoBehaviour
{
    public bool canMove=true;
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Movement")]
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private float maxSpeedNormal = 6f;
    [SerializeField] private float maxSpeedWithGrandma = 4f;
    [SerializeField] private float speedOnSafeWay = 1.5f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Refs")]
    [SerializeField] private Transform body;

    private Rigidbody rb;

    private int playerId;
    private Vector2 moveInput;
    private Vector3 velocity;

    private Transform grandma;
    private bool hasGrandma;
    float maxSpeed = 0;
    private void Awake()
    {
        maxSpeed=maxSpeedNormal;
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        playerId = playerInput.playerIndex;
        Debug.Log($"Player ID: {playerId}");
    }
    private void Start() {
        if(playerId==0)
        {
          rb.position = GrandmaGameManager.Instance.player1ResetPoint.position;
            GrandmaGameManager.Instance.player1=body;
        }
        else
        {
           rb.position = GrandmaGameManager.Instance.player2ResetPoint.position;
           GrandmaGameManager.Instance.player2=body;
        }
  
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    Vector2 rotInput=new Vector2();
    public void OnRotate(InputAction.CallbackContext ctx)
    {
        rotInput = ctx.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        if(!canMove)return;
        float dt = Time.fixedDeltaTime;

        // Target velocity
        Vector3 targetVel = new Vector3(moveInput.x, 0f, moveInput.y) * (((SafeWay)?speedOnSafeWay:1) *maxSpeed);

        // Accel / Decel
        if (moveInput.sqrMagnitude > 0.01f)
            velocity = Vector3.MoveTowards(velocity, targetVel, acceleration * dt);
        else
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * dt);

        // Move  
        rb.MovePosition(rb.position + velocity * dt);

        // Rotate  
        Vector3 lookDir = new Vector3(rotInput.x,0,rotInput.y);
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
            body.rotation = Quaternion.Slerp(body.rotation, targetRot, rotationSpeed * dt);

            // if (hasGrandma && grandma)
            //     grandma.rotation = body.rotation;
        }
 
    }
    Transform grandmaEnemy;
     
    void takeGrandma()
    {
        if(grandmaEnemy==null)return;
        if(grandmaEnemy.parent!=null)
        {grandmaEnemy.parent.GetComponent<PlayerMovementRoadGame>().grandmaTakeDown();}
        GrandmaProperty gp = grandmaEnemy.GetComponent<GrandmaProperty>();
        grandma = grandmaEnemy;
        grandma.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gp.canMove=false;
        if (grandma)
            grandma.rotation = Quaternion.identity;
        hasGrandma = true;
        gp.side=playerId;
        gp.setPlayerParent();
        // grandma.GetComponent<Rigidbody>().Kinematic.true;
        maxSpeed=maxSpeedWithGrandma;
        grandma.SetParent(transform);
        grandma.localPosition = new Vector3(1.2f, 0f, 0f);
    }
    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("car"))
        {
            canMove=false;
            GrandmaGameManager.Instance.ResetPlayerPosition(transform,playerId);
            if(hasGrandma)       
                GrandmaGameManager.Instance.ResetGrandmaPosition(grandma,playerId);
                
            GrandmaGameManager.Instance.AddPlayerToDied();
            grandmaTakeDown();
            
        }
        else if(hasGrandma&&other.CompareTag("side")
        &&other.name!=playerId.ToString())
        {
            // grandmaTakeDown();
        }
        else if(other.CompareTag("SafeWay"))
        {
 
            SafeWay=true;
        }
        else if (!hasGrandma&&other.CompareTag("grandma"))
        {
            grandmaEnemy=other.transform;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("SafeWay"))
        {
 
            SafeWay=false;
        }
        else if (other.CompareTag("grandma")&&grandmaEnemy==other.transform)
        {
            grandmaEnemy=null;
        }
    }
    bool SafeWay;
    public void grandmaDown(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
        if(!hasGrandma)
        {
            takeGrandma();
            
            return;
        }
        hasGrandma=false;
        grandma.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotation;
        grandma.SetParent(null);
        maxSpeed=maxSpeedNormal;
        grandma.GetComponent<GrandmaProperty>().canMove=true;
        grandma.GetComponent<GrandmaProperty>().setPlayerParent();
        
        }
    }
    public void grandmaTakeDown()
    {
        if(!hasGrandma)return;
        hasGrandma=false;
        grandma.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotation;
        grandma.SetParent(null);
        maxSpeed=maxSpeedNormal;
        grandma.GetComponent<GrandmaProperty>().canMove=true;
        grandma.GetComponent<GrandmaProperty>().setPlayerParent();
    }
    // void grandmaWin()
    // {
    //     hasGrandma=false;
    //     grandma.GetComponent<Rigidbody>().constraints =
    //         RigidbodyConstraints.FreezePositionY |
    //         RigidbodyConstraints.FreezeRotation;
    //     grandma.SetParent(null);
    //     maxSpeed=maxSpeedNormal;
    // }
}
