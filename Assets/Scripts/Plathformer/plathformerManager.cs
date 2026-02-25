using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;

public class plathformerManager : MonoBehaviour
{
    public static plathformerManager Instance { get; private set; }
    [SerializeField] private Transform player1,player2;
    [SerializeField] private Camera cam;
    
    [SerializeField] private List<SplineContainer> playerSplines1 = new List<SplineContainer>();
    [SerializeField] private List<SplineContainer> playerSplines2 = new List<SplineContainer>();
    [SerializeField] private List<SplineContainer> playerSplines3 = new List<SplineContainer>();
    [SerializeField] private List<SplineContainer> playerSplines4 = new List<SplineContainer>();

    [SerializeField] private List<GameObject> carObj = new();
    [SerializeField] private int CarSpawnCount = 10;  
    [SerializeField] private GameObject clearCars;
    [SerializeField] private List<GameObject> coins = new();
    [SerializeField] private List<GameObject> coinsActive = new();
    [SerializeField] private Text scoreText1, scoreText2;
    [SerializeField] private Image lightRoad1, lightRoad2, lightRoad3, lightRoad4;
    [SerializeField] private Image safeWay1, safeWay2, safeWay3, safeWay4;
    [SerializeField] private Transform colliderRoad1, colliderRoad2, colliderRoad3, colliderRoad4;
    [SerializeField] private int score1, score2;
    private Dictionary<Image, Coroutine> fadeDict = new Dictionary<Image, Coroutine>();
    [SerializeField] private float fadeDuration = 0.5f;  
    private List<CrossRoadCar> playerCars1 = new List<CrossRoadCar>();
    private List<CrossRoadCar> playerCars2 = new List<CrossRoadCar>();
    private List<CrossRoadCar> playerCars3 = new List<CrossRoadCar>();
    private List<CrossRoadCar> playerCars4 = new List<CrossRoadCar>();

    [Header("Camera Follow Settings")]
    [SerializeField] private float cameraFollowSpeed = 6f;
    [SerializeField] private float fovMin = 50f;
    [SerializeField] private float fovMax = 80f;
    [SerializeField] private float distanceMinForFov = 1f;
    [SerializeField] private float distanceMaxForFov = 20f;
    [SerializeField] private float fovSmoothSpeed = 5f;
    private float _cachedCamY;
    private float _cachedCamZ;
    private float _initialCamX;
    private bool _camInit = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(spawnRandomCar());
        _SpawnCoin();
        _SpawnCoin();
        _SpawnCoin();
    }

    int r = 0;
void checkCarBugs()
{
    if (clearCars == null) return;

    // گرفتن همه کلایدرها داخل clearCars
    Collider[] collidersToCheck = clearCars.GetComponentsInChildren<Collider>();

    foreach (var col in collidersToCheck)
    {
        if (col == null) continue;

        Collider[] hits = Physics.OverlapBox(
            col.transform.position,
            col.transform.localScale * 0.5f,
            col.transform.rotation
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Car"))
            {
                CrossRoadCar car = hit.GetComponent<CrossRoadCar>();
                if (car != null)
                {
                    // حذف از لیست‌ها
                    playerCars1.Remove(car);
                    playerCars2.Remove(car);
                    playerCars3.Remove(car);
                    playerCars4.Remove(car);

                    Destroy(hit.gameObject);
                }
            }
        }
    }
}
    IEnumerator spawnRandomCar()
    {
        for (int c = 0; c < 4; c++)
        {
            InstanceCar(0);
            InstanceCar(1);
            InstanceCar(2);
            InstanceCar(3);
            yield return new WaitForSeconds(0.3f);
        }
        while (true)  
        {
            checkCarBugs();
            for (int c = 0; c < 5; c++)
            {
                InstanceCar(r);
                InstanceCar(r + 2); 
                yield return new WaitForSeconds(0.3f);
            }
            r = (r == 0) ? 1 : 0;

            if (r == 0)
            {
                if (playerSplines1.Count > 1)
                    foreach (var car in playerCars1) car.TakeMove(playerSplines1[1]);
                if (playerSplines3.Count > 1)
                    foreach (var car in playerCars3) car.TakeMove(playerSplines3[1]);

                playerCars1.Clear();
                playerCars3.Clear();

                lightRoad1.color = Color.green;
                lightRoad2.color = Color.green;
                lightRoad3.color = Color.red;
                lightRoad4.color = Color.red;

                colliderRoad1.transform.position = new Vector3(colliderRoad1.transform.position.x, 0, colliderRoad1.transform.position.z);
                colliderRoad2.transform.position = new Vector3(colliderRoad2.transform.position.x, 0, colliderRoad2.transform.position.z);
                colliderRoad3.transform.position = new Vector3(colliderRoad3.transform.position.x, 100, colliderRoad3.transform.position.z);
                colliderRoad4.transform.position = new Vector3(colliderRoad4.transform.position.x, 100, colliderRoad4.transform.position.z);

                SetAlpha(safeWay1, 0.3f);
                SetAlpha(safeWay2, 0.3f);
                SetAlpha(safeWay3, 1);
                SetAlpha(safeWay4, 1);
            }
            else
            {
                if (playerSplines2.Count > 1)
                    foreach (var car in playerCars2) car.TakeMove(playerSplines2[1]);
                if (playerSplines4.Count > 1)
                    foreach (var car in playerCars4) car.TakeMove(playerSplines4[1]);

                playerCars4.Clear();
                playerCars2.Clear();

                lightRoad1.color = Color.red;
                lightRoad2.color = Color.red;
                lightRoad3.color = Color.green;
                lightRoad4.color = Color.green;

                colliderRoad1.transform.position = new Vector3(colliderRoad1.transform.position.x, 100, colliderRoad1.transform.position.z);
                colliderRoad2.transform.position = new Vector3(colliderRoad2.transform.position.x, 100, colliderRoad2.transform.position.z);
                colliderRoad3.transform.position = new Vector3(colliderRoad3.transform.position.x, 0, colliderRoad3.transform.position.z);
                colliderRoad4.transform.position = new Vector3(colliderRoad4.transform.position.x, 0, colliderRoad4.transform.position.z);

                SetAlpha(safeWay1, 1);
                SetAlpha(safeWay2, 1);
                SetAlpha(safeWay3, 0.3f);
                SetAlpha(safeWay4, 0.3f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int c = 0; c < CarSpawnCount; c++)
            {
                InstanceCarAuto(r);
                InstanceCarAuto(r + 2); 
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void _AddScore(int PlayerId,GameObject g)
    {
        _SpawnCoin();
        coinsActive.Remove(g);
        coins.Add(g);
        if (PlayerId == 0)
        {
            score1++;
            scoreText1.text = score1.ToString();
        }
        else
        {
            score2++;
            scoreText2.text = score2.ToString();
        }
    }

    public void _RemoveScore(int PlayerId) { }

    public void _checkWin(int playerID)
    {
        if (playerID == 0)
        {
            if (score1 > 9) Debug.Log("player 1 win");
        }
        else
        {
            if (score2 > 9) Debug.Log("player 2 win");
        }
    }

    void InstanceCarAuto(int playerId)
    {
        GameObject g = Instantiate(carObj[Random.Range(0, carObj.Count)], new Vector3(100, 100, 100), Quaternion.identity);
        CrossRoadCar c = g.GetComponent<CrossRoadCar>();
        Destroy(g,10);

        c.setDefault(playerId);

        if (playerId == 0) { c.TakeMove(playerSplines1[0]); c.TakeMove(playerSplines1[1]); }
        else if (playerId == 1) { c.TakeMove(playerSplines2[0]); c.TakeMove(playerSplines2[1]); }
        else if (playerId == 2) { c.TakeMove(playerSplines3[0]); c.TakeMove(playerSplines3[1]); }
        else if (playerId == 3) { c.TakeMove(playerSplines4[0]); c.TakeMove(playerSplines4[1]); }
    }

    void InstanceCar(int playerIdTurn)
    {
        GameObject g = Instantiate(carObj[Random.Range(0, carObj.Count)], new Vector3(100, 100, 100), Quaternion.identity);
        CrossRoadCar c = g.GetComponent<CrossRoadCar>();
        Destroy(g,14);
        c.setDefault(playerIdTurn);

        if (playerIdTurn == 0) { playerCars1.Add(c); if (playerSplines1.Count > 0) c.TakeMove(playerSplines1[0]); }
        else if (playerIdTurn == 1) { playerCars2.Add(c); if (playerSplines2.Count > 0) c.TakeMove(playerSplines2[0]); }
        else if (playerIdTurn == 2) { playerCars3.Add(c); if (playerSplines3.Count > 0) c.TakeMove(playerSplines3[0]); }
        else if (playerIdTurn == 3) { playerCars4.Add(c); if (playerSplines4.Count > 0) c.TakeMove(playerSplines4[0]); }
    }

    public void _SpawnCoin()
    {
        if (coins.Count == 0) return;
        int i=Random.Range(0, coins.Count);
        coins[i].SetActive(true);
        coinsActive.Add(coins[i]);
        coins.RemoveAt(i);
    }

    void SetAlpha(Image img, float targetAlpha)
    {
        if (img == null) return;
        if (fadeDict.ContainsKey(img)) { if (fadeDict[img] != null) StopCoroutine(fadeDict[img]); fadeDict.Remove(img); }
        fadeDict[img] = StartCoroutine(FadeAlpha(img, targetAlpha));
    }

    IEnumerator FadeAlpha(Image img, float target)
    {
        if (img == null) yield break;
        Color c = img.color;
        float start = c.a;
        float t = 0f;
        if (fadeDuration <= 0f) { c.a = target; img.color = c; if (fadeDict.ContainsKey(img)) fadeDict.Remove(img); yield break; }
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, target, t / fadeDuration);
            img.color = c;
            yield return null;
        }
        c.a = target;
        img.color = c;
        if (fadeDict.ContainsKey(img)) fadeDict.Remove(img);
    }

    void LateUpdate()
{
    if (player1 == null || player2 == null) return;
    if (cam == null) cam = Camera.main;
    if (cam == null) return;

    if (!_camInit)
    {
        _cachedCamY = cam.transform.position.y;
        _cachedCamZ = cam.transform.position.z;
        _initialCamX = cam.transform.position.x;
        _camInit = true;
    }

    Vector3 pos1 = player1.position;
    Vector3 pos2 = player2.position;

    // --- حرکت محور X ---
    float targetX = (pos1.x + pos2.x) * 0.5f;
    targetX = Mathf.Clamp(targetX, _initialCamX - 10f, _initialCamX + 10f); 
    float newX = Mathf.Lerp(cam.transform.position.x, targetX, Time.deltaTime * cameraFollowSpeed);
    cam.transform.position = new Vector3(newX, _cachedCamY, _cachedCamZ);

    // --- فاصله XZ برای زوم ---
    float distX = Mathf.Abs(pos1.x - pos2.x);
    float distZ = Mathf.Abs(pos1.z - pos2.z);

    float weightedDist = distX * 0.7f + distZ * 0.3f; // 70% X, 30% Z

    float t = Mathf.InverseLerp(distanceMinForFov, distanceMaxForFov, weightedDist);
    float targetFov = Mathf.Lerp(fovMin, fovMax, t);
    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovSmoothSpeed);
}
}