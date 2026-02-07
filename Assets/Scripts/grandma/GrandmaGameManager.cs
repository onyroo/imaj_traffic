using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CarSpawnData
{
    public GameObject prefab;
    [Min(0f)] public float chance;
}

public class GrandmaGameManager : MonoBehaviour
{
    public static GrandmaGameManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private int grandmaSpawnCount = 3;
    [SerializeField] private GameObject grandmaPrefab;


    [Header("Car Settings")]
    [SerializeField] private List<CarSpawnData> carSpawnDatas = new();
    [SerializeField] private List<Transform> roads = new();
    [SerializeField] private Vector2 carSpawnDelayRange = new Vector2(1f, 3f);

    [Header("Spawn Points")]
    public Transform sideSpawnPointA;
    public Transform sideSpawnPointB;
    [SerializeField] private float cooldownSpawn = 3f;

    [Header("Player Reset Points")]
    public Transform player1ResetPoint;
    public Transform player2ResetPoint;

    [Header("UI")]
    [SerializeField] private Text bluePlayerText;
    [SerializeField] private Text redPlayerText;
    [SerializeField] private Camera cam;

    private int bluePlayerScore;
    private int redPlayerScore;

    public Transform player1, player2;
    [SerializeField] private float limitPosCamera;
    [SerializeField] private float camMoveSpeed;
    [SerializeField] private float limitSizeCamera;
    [SerializeField] private float camSizeSpeed;

    Vector3 startCamPos;
    float startCamSize;
    
    private void Awake()
    {
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

        startCamSize = cam.orthographicSize;
        startCamPos = cam.transform.position;
        // anim=GetComponent<Animator>();
        
    }

    private void Update()
    {
        if (!player1 || !player2) return;

        float combinedDir = Mathf.Clamp(player1.right.z + player2.right.z, -1, 1);

        float camPosAxis = Mathf.Clamp(combinedDir * limitPosCamera, -limitPosCamera, limitPosCamera);
        float camSize = Mathf.Clamp(
            (Mathf.Abs(player1.right.z) + Mathf.Abs(player2.right.z)) * limitSizeCamera,
            0,
            limitSizeCamera
        );

        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            startCamPos + new Vector3(-camPosAxis, 0, 0),
            camMoveSpeed * Time.deltaTime
        );

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            startCamSize + camSize,
            camSizeSpeed * Time.deltaTime
        );
    }

    private IEnumerator CarGenerator()
    {
        while (true)
        {
            if (carSpawnDatas.Count > 0 && roads.Count > 0)
            {
                GameObject prefab = GetRandomCarByChance();
                if (prefab != null)
                {
                    Transform road = roads[Random.Range(0, roads.Count)];
                    GameObject car = Instantiate(prefab, road.position, road.rotation, road);
                    Destroy(car, 50f);
                }
            }

            yield return new WaitForSeconds(Random.Range(carSpawnDelayRange.x, carSpawnDelayRange.y));
        }
    }

    private GameObject GetRandomCarByChance()
    {
        float totalChance = 0f;
        foreach (var data in carSpawnDatas)
        {
            if (data.prefab != null && data.chance > 0)
                totalChance += data.chance;
        }

        if (totalChance <= 0f) return null;

        float rand = Random.Range(0f, totalChance);
        float current = 0f;

        foreach (var data in carSpawnDatas)
        {
            if (data.prefab == null || data.chance <= 0) continue;

            current += data.chance;
            if (rand <= current)
                return data.prefab;
        }

        return null;
    }

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
        GameObject g = Instantiate(grandmaPrefab, point.position, grandmaPrefab.transform.rotation);
        GrandmaProperty gp = g.GetComponent<GrandmaProperty>();
        if (gp) gp.side = side;
    }

    List<Coroutine> cldown = new();
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

        player.position = side == 0 ? player1ResetPoint.position : player2ResetPoint.position;
        onePlayerDead = false;

        player.GetChild(0).gameObject.SetActive(true);
        player.GetComponent<PlayerMovementRoadGame>().canMove = true;
    }

    public void AddPlayerToDied()
    {
        if (onePlayerDead)
        {
            foreach (var c in cldown)
                if (c != null) StopCoroutine(c);

            cldown.Clear();

            player1.parent.position = player1ResetPoint.position;
            player2.parent.position = player2ResetPoint.position;

            player1.parent.GetComponent<PlayerMovementRoadGame>().canMove = true;
            player2.parent.GetComponent<PlayerMovementRoadGame>().canMove = true;

            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);

            onePlayerDead = false;
            return;
        }

        onePlayerDead = true;
        Invoke(nameof(ResetDie), 0.5f);
    }

    void ResetDie()
    {
        onePlayerDead = false;
    }

    public void ResetGrandmaPosition(Transform grandma, int side)
    {
        if(side==0)
        {
          player1.parent.GetComponent<PlayerMovementRoadGame>().StartShortRumble(0.3f, 0.9f, 0.9f);  
        }
        else
        {
          player2.parent.GetComponent<PlayerMovementRoadGame>().StartShortRumble(0.3f, 0.9f, 0.9f); 
        }
         
        grandma.position = side == 0 ? sideSpawnPointA.position : sideSpawnPointB.position;
    }

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
        if (bluePlayerText) bluePlayerText.text = $"Blue: {bluePlayerScore}";
        if (redPlayerText) redPlayerText.text = $"Red: {redPlayerScore}";
    }
}
