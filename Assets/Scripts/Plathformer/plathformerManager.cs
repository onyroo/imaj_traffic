using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;
public class plathformerManager : MonoBehaviour
{
    public static plathformerManager Instance { get; private set; }
    [SerializeField]private List<SplineContainer> playerSplines1 = new List<SplineContainer>();
    [SerializeField]private List<SplineContainer> playerSplines2 = new List<SplineContainer>();
    [SerializeField]private List<SplineContainer> playerSplines3 = new List<SplineContainer>();
    [SerializeField]private List<SplineContainer> playerSplines4 = new List<SplineContainer>();
    [SerializeField] private List<GameObject> carObj=new ();
    [SerializeField] private float CarSpawnCount=10;

    [SerializeField]private List<GameObject> coins=new();
    private void Awake() {
        Instance = this;
    }
    void Start()
    {
        StartCoroutine(spawnRandomCar());
    }
 
    // void Update()
    // {
        
    // }
    int r=0;
    IEnumerator spawnRandomCar()
    {
        // int r=Random.Range(0,2);
        r=(r==0)?1:0;
        int c=0;
        while(c<CarSpawnCount)
        {
            InstanceCar(r);
            InstanceCar(r+2);
            c++;
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(spawnRandomCar());
    }
    void InstanceCar(int playerIdTurn)
    {
        GameObject g = Instantiate(carObj[Random.Range(0,carObj.Count)],new Vector3(100,100,100),Quaternion.identity);
        CrossRoadCar c=g.GetComponent<CrossRoadCar>();
        c.setDefault(playerIdTurn);

        if(playerIdTurn==0)
        {
            // playerCars1.Add(g);
            c.TakeMove(playerSplines1[0]);
            c.TakeMove(playerSplines1[1]);
            
        }
        else if(playerIdTurn==1)
        {
            // playerCars2.Add(g);
            c.TakeMove(playerSplines2[0]);
            c.TakeMove(playerSplines2[1]);
        }
        else if(playerIdTurn==2)
        {
            // playerCars2.Add(g);
            c.TakeMove(playerSplines3[0]);
            c.TakeMove(playerSplines3[1]);
        }
        else if(playerIdTurn==3)
        {
            // playerCars2.Add(g);
            c.TakeMove(playerSplines4[0]);
            c.TakeMove(playerSplines4[1]);
        }
    }
    public void _SpawnCoin()
    {
        coins[Random.Range(0,coins.Count)].SetActive(true);
    }

}
