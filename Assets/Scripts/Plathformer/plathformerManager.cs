using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;

public class plathformerManager : MonoBehaviour
{
    public static plathformerManager Instance { get; private set; }

    [SerializeField] private List<SplineContainer> playerSplines1 = new List<SplineContainer>();
    [SerializeField] private List<SplineContainer> playerSplines2 = new List<SplineContainer>();
    [SerializeField] private List<SplineContainer> playerSplines3 = new List<SplineContainer>();
    [SerializeField] private List<SplineContainer> playerSplines4 = new List<SplineContainer>();

    [SerializeField] private List<GameObject> carObj = new();
    [SerializeField] private int CarSpawnCount = 10; // int بهتر است
    [SerializeField] private GameObject clearCars;
    [SerializeField] private List<GameObject> coins = new();
    [SerializeField] private Text scoreText1, scoreText2;
    [SerializeField] private Image lightRoad1, lightRoad2, lightRoad3, lightRoad4;
    [SerializeField] private int score1, score2;

    // <-- اینجا لیست‌های مخصوص نگهداری کامپوننت ماشین‌ها
    private List<CrossRoadCar> playerCars1 = new List<CrossRoadCar>();
    private List<CrossRoadCar> playerCars2 = new List<CrossRoadCar>();
    private List<CrossRoadCar> playerCars3 = new List<CrossRoadCar>();
    private List<CrossRoadCar> playerCars4 = new List<CrossRoadCar>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(spawnRandomCar());
    }

    int r = 0;

    IEnumerator spawnRandomCar()
    {
        clearCars.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        clearCars.SetActive(false);
        
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
 
            for (int c = 0; c < 5; c++)
            {
                InstanceCar(r);
                InstanceCar(r + 2); 
                yield return new WaitForSeconds(0.3f);
            }
            r = (r == 0) ? 1 : 0;
             
            
            // yield return new WaitForSeconds(1f);

            if (r == 0)
            {
                if (playerSplines1.Count > 1)
                {
                    foreach (var car in playerCars1)
                        car.TakeMove(playerSplines1[1]);
                }
                if (playerSplines3.Count > 1)
                {
                    foreach (var car in playerCars3)
                        car.TakeMove(playerSplines3[1]);
                }

                playerCars1.Clear();
                playerCars3.Clear();

                lightRoad1.color = Color.green;
                lightRoad2.color = Color.green;
                lightRoad3.color = Color.red;
                lightRoad4.color = Color.red;
            }
            else
            {
                if (playerSplines2.Count > 1)
                {
                    foreach (var car in playerCars2)
                        car.TakeMove(playerSplines2[1]);
                }
                if (playerSplines4.Count > 1)
                {
                    foreach (var car in playerCars4)
                        car.TakeMove(playerSplines4[1]);
                }

                playerCars4.Clear();
                playerCars2.Clear();

                lightRoad1.color = Color.red;
                lightRoad2.color = Color.red;
                lightRoad3.color = Color.green;
                lightRoad4.color = Color.green;
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

    public void _AddScore(int PlayerId)
    {
        _SpawnCoin();
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

    public void _RemoveScore(int PlayerId)
    {
        // اگر خواستی فعال کن
    }

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
      

        c.setDefault(playerId);

        if (playerId == 0)
        {
            c.TakeMove(playerSplines1[0]);
            c.TakeMove(playerSplines1[1]);
            
        }
        else if (playerId == 1)
        {
            c.TakeMove(playerSplines2[0]);
            c.TakeMove(playerSplines2[1]);
        }
        else if (playerId == 2)
        {
            c.TakeMove(playerSplines3[0]);
            c.TakeMove(playerSplines3[1]);
        }
        else if (playerId == 3)
        {
            c.TakeMove(playerSplines4[0]);
            c.TakeMove(playerSplines4[1]);
        }
    }
    void InstanceCar(int playerIdTurn)
    {
        GameObject g = Instantiate(carObj[Random.Range(0, carObj.Count)], new Vector3(100, 100, 100), Quaternion.identity);
        CrossRoadCar c = g.GetComponent<CrossRoadCar>();
      

        c.setDefault(playerIdTurn);

        if (playerIdTurn == 0)
        {
            playerCars1.Add(c);
            if (playerSplines1.Count > 0) c.TakeMove(playerSplines1[0]);
        }
        else if (playerIdTurn == 1)
        {
            playerCars2.Add(c);
            if (playerSplines2.Count > 0) c.TakeMove(playerSplines2[0]);
        }
        else if (playerIdTurn == 2)
        {
            playerCars3.Add(c);
            if (playerSplines3.Count > 0) c.TakeMove(playerSplines3[0]);
        }
        else if (playerIdTurn == 3)
        {
            playerCars4.Add(c);
            if (playerSplines4.Count > 0) c.TakeMove(playerSplines4[0]);
        }
    }

    public void _SpawnCoin()
    {
        if (coins.Count == 0) return;
        coins[Random.Range(0, coins.Count)].SetActive(true);
    }
}