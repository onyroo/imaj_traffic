using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRoadGame : MonoBehaviour
{
    public bool canMove = true;
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Collider cl;
    [Header("Movement")]
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private float maxSpeedNormal = 6f;
    [SerializeField] private float maxSpeedWithGrandma = 4f;
    [SerializeField] private float speedOnSafeWay = 1.5f;

    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private Animator anim;
    private int countCall;

    [Header("Refs")]
    [SerializeField] private Transform body;

    private Rigidbody rb;

    private int playerId;
    private Vector2 moveInput;
    private Vector3 velocity;

    private Transform grandma;
    private bool hasGrandma;
    float maxSpeed = 0;

    bool SafeWay;
    Coroutine idleRumbleCoroutine;
    Coroutine shortRumbleCoroutine;

    private void Awake()
    {
        StopAllCoroutines();
        maxSpeed = maxSpeedNormal;
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        playerId = playerInput.playerIndex;
        Debug.Log($"Player ID: {playerId}");
    }

    private void Start()
    {
        if (playerId == 0)
        {
            rb.position = GrandmaGameManager.Instance.player1ResetPoint.position;
            GrandmaGameManager.Instance.player1 = body;
        }
        else
        {
            rb.position = GrandmaGameManager.Instance.player2ResetPoint.position;
            GrandmaGameManager.Instance.player2 = body;
        }
        // anim=GetComponent<Animator>();
        StartIdleRumbleIfAllowed();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

  
    private void FixedUpdate()
    {
        if (!canMove) return;
        float dt = Time.fixedDeltaTime;

        Vector3 targetVel = new Vector3(moveInput.x, 0f, moveInput.y) * (((SafeWay) ? speedOnSafeWay : 1) * maxSpeed);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            velocity = Vector3.MoveTowards(velocity, targetVel, acceleration * dt);
            anim.SetInteger("walk",1);
            if(hasGrandma)
                grandmaAnim.SetInteger("walk",1);

        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * dt);
            anim.SetInteger("walk",0);
            if(hasGrandma)
                grandmaAnim.SetInteger("walk",0);
        }
            
        rb.linearVelocity=Vector3.zero;
        rb.MovePosition(rb.position + velocity * dt);

        Vector3 lookDir = new Vector3(moveInput.x, 0, moveInput.y);
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
            body.rotation = Quaternion.Slerp(body.rotation, targetRot, rotationSpeed * dt);
            if(grandma)
                grandma.rotation=body.rotation;
        }
    }

    Transform grandmaEnemy;
    Animator grandmaAnim;
    [SerializeField]private float grandmaTheftCoolown=1;
    public bool canTheft=true;
    void doCoolDownGrandaTheft()
    {
        canTheft=true;
    }
    void takeGrandma()
    {
        if (grandmaEnemy == null) return;
        if (grandmaEnemy.parent != null)
        {
            if(!grandmaEnemy.parent.GetComponent<PlayerMovementRoadGame>().canTheft)
            {
                return;
            }
            canTheft=false;
            Invoke("doCoolDownGrandaTheft",grandmaTheftCoolown);
            grandmaEnemy.parent.GetComponent<PlayerMovementRoadGame>().grandmaTakeDown();
        }
        GrandmaProperty gp = grandmaEnemy.GetComponent<GrandmaProperty>();
        grandmaAnim=gp.animReturn();
        grandma = grandmaEnemy;
        grandma.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gp.canMove = false;
        if (playerId==1)
            grandma.rotation = Quaternion.Euler(0,0,0);
        else
            grandma.rotation = Quaternion.Euler(0,180,0);

        hasGrandma = true;
        gp.side = playerId;
            cl.enabled=true;

        gp.setPlayerParent();
        maxSpeed = maxSpeedWithGrandma;
        grandma.SetParent(transform);
        grandma.localPosition = new Vector3(1.2f, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("car"))
        {
            canMove = false;
            GrandmaGameManager.Instance.ResetPlayerPosition(transform, playerId);
            if (hasGrandma)
                GrandmaGameManager.Instance.ResetGrandmaPosition(grandma
                , grandma.GetComponent<GrandmaProperty>().lastSide
                ,playerId);

            GrandmaGameManager.Instance.AddPlayerToDied(gameObject);
 
            StartShortRumble(0.5f, 0.9f, 0.9f); // intense 1s rumble

            grandmaTakeDown();
        }
        else if (other.CompareTag("side"))
        {
            countCall++;
            StopIdleRumble(); // in side -> no idle rumble
            if(other.name==playerId.ToString())
            {

            if(hasGrandma&&
            other.name!=grandma.GetComponent<GrandmaProperty>().lastSide.ToString())
            {
            // Debug.Log(grandma.GetComponent<GrandmaProperty>().side.ToString());
            grandma.GetComponent<GrandmaProperty>().lastSide=playerId;
            GrandmaGameManager.Instance.AddScore(playerId,10+((safeWayScore)?5:0));
            }
 
            }
            safeWayScore=true;
            
        }
        else if (other.CompareTag("SafeWay"))
        {
            countCall++;
            SafeWay = true;
            StopIdleRumble(); // safe way -> no idle rumble
        }
        else if (!hasGrandma && other.CompareTag("grandma"))
        {
            grandmaEnemy = other.transform;
        }
    }
    bool safeWayScore;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeWay"))
        {
            countCall = Mathf.Max(0, countCall - 1);
            SafeWay = false;
            if (countCall == 0)
            {
                StartIdleRumbleIfAllowed();
                if(hasGrandma)
                {
                  safeWayScore=false;  
                }
            }
        }
        else if (other.CompareTag("grandma") && grandmaEnemy == other.transform)
        {
            grandmaEnemy = null;
        }
        else if (other.CompareTag("side"))
        {
            countCall = Mathf.Max(0, countCall - 1);
            if (countCall == 0)
            {
                StartIdleRumbleIfAllowed();
                if(hasGrandma)
                {
                  safeWayScore=false;  
                }
            }
        }
    }

    public void grandmaDown(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!hasGrandma)
            {
                takeGrandma();
                return;
            }
            hasGrandma = false;
            grandmaAnim.SetInteger("walk",1);
            grandma.GetComponent<Rigidbody>().constraints =
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotation;
            grandma.SetParent(null);
            maxSpeed = maxSpeedNormal;
            grandma.GetComponent<GrandmaProperty>().canMove = true;
            grandma.GetComponent<GrandmaProperty>().setPlayerParent();
            grandma=null;
            cl.enabled=false;

        }
    }

    public void grandmaTakeDown()
    {
        if (!hasGrandma) return;
        hasGrandma = false;
        grandma.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotation;
        grandma.SetParent(null);

        maxSpeed = maxSpeedNormal;
        grandma.GetComponent<GrandmaProperty>().canMove = true;
        grandma.GetComponent<GrandmaProperty>().setPlayerParent();
        

        grandmaAnim.SetInteger("walk",1);
        grandma=null;
        cl.enabled=false;
    }

    // ---------- RUMBLE / CONTROLLER HELPERS ----------

    private Gamepad GetPlayerGamepad()
{
    if (playerInput != null)
    {
        foreach (var dev in playerInput.devices)
        {
            if (dev is Gamepad gp)
                return gp;
        }
    }

    // fallback: pick first available gamepad
    if (Gamepad.all.Count > 0)
    {
        int idx = Mathf.Clamp(playerId, 0, Gamepad.all.Count - 1);
        return Gamepad.all[idx];
    }

    return Gamepad.current;
}

    // start gentle idle rumble if player is not in safe/side and not currently short-rumbling
    private void StartIdleRumbleIfAllowed()
    {
        if (SafeWay || countCall > 0||!canMove) return;
        if (idleRumbleCoroutine != null) return;
        var pad = GetPlayerGamepad();
        if (pad == null) return;
        idleRumbleCoroutine = StartCoroutine(IdleRumble(pad));
    }

    private void StopIdleRumble()
    {
        if (idleRumbleCoroutine != null)
        {
            StopCoroutine(idleRumbleCoroutine);
            idleRumbleCoroutine = null;
        }
        var pad = GetPlayerGamepad();
        if (pad != null) pad.SetMotorSpeeds(0f, 0f);
    }

    public void StartShortRumble(float duration = 0.3f, float low = 0.8f, float high = 0.8f)
    {
        StopIdleRumble();
        var pad = GetPlayerGamepad();
        if (pad == null) return;
        if (shortRumbleCoroutine != null)
        {
            StopCoroutine(shortRumbleCoroutine);
            shortRumbleCoroutine = null;
        }
        shortRumbleCoroutine = StartCoroutine(ShortRumble(pad, duration, low, high));
    }

    private IEnumerator IdleRumble(Gamepad pad)
    {
        while (true)
        {
            pad.SetMotorSpeeds(0.05f, 0.05f);
            yield return null;
        }
    }

    private IEnumerator ShortRumble(Gamepad pad, float duration, float lowFreq, float highFreq)
    {
        float timer = 0f;
        // optionally fade in a bit for smoothness
        float ramp = Mathf.Min(0.12f, duration * 0.2f);
        while (timer < duration)
        {
            float t = timer / duration;
            float rampFactor = (timer < ramp) ? (timer / ramp) : 1f;
            pad.SetMotorSpeeds(lowFreq * rampFactor, highFreq * rampFactor);
            timer += Time.deltaTime;
            yield return null;
        }
        pad.SetMotorSpeeds(0f, 0f);
        shortRumbleCoroutine = null;

        // after a short rumble, resume idle if allowed
        // StartIdleRumbleIfAllowed();
    }
}
