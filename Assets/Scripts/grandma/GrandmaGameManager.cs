using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrandmaGameManager : MonoBehaviour
{
    public static GrandmaGameManager Instance { get; private set; }
 
    [Header("Spawn Settings")]
    [SerializeField] private int grandmaSpawnCount = 3;
    [SerializeField] private GameObject grandmaPrefab;

    [Header("Car Settings")]
    [SerializeField] private List<GameObject> cars = new();
    [SerializeField] private List<Transform> roads = new();
    [SerializeField] private Vector2 carSpawnDelayRange = new Vector2(1f, 3f);

    [Header("Spawn Points")]
    public Transform sideSpawnPointA;
    public Transform sideSpawnPointB;
    [SerializeField] private float cooldownSpawn=3;
    

    [Header("Player Reset Points")]
    public Transform player1ResetPoint;
    public Transform player2ResetPoint;

    [Header("UI")]
    [SerializeField] private Text bluePlayerText;
    [SerializeField] private Text redPlayerText;
    [SerializeField] private Camera cam;

    private int bluePlayerScore;
    private int redPlayerScore;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        redPlayerScore = grandmaSpawnCount;
        bluePlayerScore = grandmaSpawnCount;

        SpawnGrandmas();
        UpdateScoreUI();

        StartCoroutine(CarGenerator());
        startCamSize=cam.orthographicSize;
        startCamPos=cam.transform.position;
    }
    public Transform player1,player2;
    [SerializeField] private float limitPosCamera;
    [SerializeField] private float camMoveSpeed;
    [SerializeField] private float limitSizeCamera;
    [SerializeField] private float camSizeSpeed;
    Vector3 startCamPos=new Vector3();
    float startCamSize;
    
    private void Update()
    {
        if (!player1 || !player2) return;

        // float sign1 = Mathf.Sign(player1.right.z);
        // float sign2 = Mathf.Sign(player2.right.z);
        
        float combinedDir = Mathf.Clamp(player1.right.z+player2.right.z,-1,1);

        float camPosAxis = Mathf.Clamp(
            combinedDir * limitPosCamera,
            -limitPosCamera,
            limitPosCamera
        );

        float camSize = Mathf.Clamp(
            (Mathf.Abs(player1.right.z)+Mathf.Abs(player2.right.z)) * limitSizeCamera,
            0,
            limitSizeCamera
        );

        Vector3 targetPos = startCamPos + new Vector3(-camPosAxis, 0f, 0f);
        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            targetPos,
            camMoveSpeed * Time.deltaTime
        );

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            startCamSize + camSize,
            camSizeSpeed * Time.deltaTime
        );
    }

 
    // CAR GENERATOR 
 
    private IEnumerator CarGenerator()
    {
        while (true)
        {
            if (cars.Count > 0 && roads.Count > 0)
            {
                GameObject carPrefab = cars[Random.Range(0, cars.Count)];
                Transform road = roads[Random.Range(0, roads.Count)];

                carPrefab=Instantiate(
                    carPrefab,
                    road.position,
                    road.rotation,
                    road
                );
                Destroy(carPrefab,100);
            }

            float delay = Random.Range(carSpawnDelayRange.x, carSpawnDelayRange.y);
            yield return new WaitForSeconds(delay);
        }
    }

 
    // GRANDMA SPAWN
 
    private void SpawnGrandmas()
    {
        for (int i = 0; i < grandmaSpawnCount; i++)
        {
            SpawnGrandma(sideSpawnPointA, 0);
            SpawnGrandma(sideSpawnPointB, 1);
        }
    }

    private void SpawnGrandma(Transform point, int side)
    {
        GameObject t = Instantiate(
            grandmaPrefab,
            point.position,
            grandmaPrefab.transform.rotation
        );

        GrandmaProperty gp = t.GetComponent<GrandmaProperty>();
        if (gp) gp.side = side;
    }

 
    // RESETS
 
    List<Coroutine> cldown = new List<Coroutine>();
    bool onePlayerDead;

    public void ResetPlayerPosition(Transform player, int side)
    {
        player.GetChild(0).gameObject.SetActive(false);

        player.position = new Vector3(100, 0, 100);
        cldown.Add(StartCoroutine(Cooldown(player, side)));
    }

    IEnumerator Cooldown(Transform player, int side)
    {
        yield return new WaitForSeconds(cooldownSpawn);

        player.position = (side == 0)
            ? player1ResetPoint.position
            : player2ResetPoint.position;
        onePlayerDead = false;

        player.GetChild(0).gameObject.SetActive(true);
        player.GetComponent<PlayerMovementRoadGame>().canMove=true;
    }

    public void AddPlayerToDied()
    {
        if (onePlayerDead)
        {
            foreach (Coroutine c in cldown)
            {
                if (c != null)
                    StopCoroutine(c);
            }

            cldown.Clear();

            player1.parent.position = player1ResetPoint.position;
            player2.parent.position = player2ResetPoint.position;

            onePlayerDead = false;
        player1.parent.GetComponent<PlayerMovementRoadGame>().canMove=true;
        player2.parent.GetComponent<PlayerMovementRoadGame>().canMove=true;
        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);

            return;
        }

        onePlayerDead = true;
        Invoke("ResetDie",0.5f);
    }
    void ResetDie()
    {
        onePlayerDead = false;
    }
    public void ResetGrandmaPosition(Transform grandma, int side)
    {
        grandma.position = (side == 0)
            ? sideSpawnPointA.position
            : sideSpawnPointB.position;
    }

    
    // SCORE
   
    public void AddScore(int side, int amount = 1)
    {
        if (side == 0)
            {
                bluePlayerScore -= amount;
                redPlayerScore += amount;
            }
        else
            {
                redPlayerScore -= amount;
                bluePlayerScore += amount;
            }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (bluePlayerText)
            bluePlayerText.text = $"Blue: {bluePlayerScore}";

        if (redPlayerText)
            redPlayerText.text = $"Red: {redPlayerScore}";
    }
}
