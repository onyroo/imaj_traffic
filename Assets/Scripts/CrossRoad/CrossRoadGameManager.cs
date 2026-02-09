using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
public class CrossRoadGameManager : MonoBehaviour
{
     public static CrossRoadGameManager Instance;

    [Header("UI")]
    [SerializeField] private Text bluePlayerTxt;
    [SerializeField] private Text redPlayerTxt;
    [SerializeField] private GameObject bluePlayerSprite;
    [SerializeField] private GameObject redPlayerSprite;

    [Header("Score")]
    [SerializeField] private int bluePlayerScore;
    [SerializeField] private int redPlayerScore;

    [Header("Turn Settings")]
    [SerializeField] private float timePlay = 30f;
    [SerializeField] private Vector2Int betweenTurnTime = new Vector2Int(1, 3);


    [Header("Car")]
    [SerializeField] private List<GameObject> carObj=new ();
 

    private List<float> playerTurns1 = new List<float>();
    private List<float> playerTurns2 = new List<float>();

    [SerializeField]private List<SplineContainer> playerSplines1 = new List<SplineContainer>();
    [SerializeField]private List<SplineContainer> playerSplines2 = new List<SplineContainer>();


    private List<GameObject> playerCars1 = new List<GameObject>();
    private List<GameObject> playerCars2 = new List<GameObject>();
 
    private int turnPlayer=3;
    private bool finish;

     void Awake()
    {
        Instance = this;
        TurnGenerator();
        
    }
    private void Start() {
         firstCars();
         StartCoroutine(waitForApawnCar());
        Invoke("firstCars",0.3f);
        Invoke("firstCars",0.6f);
        Invoke("firstCars",0.9f);
        Invoke("firstCars",1.2f);
        Invoke("firstCars",1.5f);
        Invoke("firstCars",1.8f);
    }
    void firstCars()
    {
        CarGenerate(0);
        CarGenerate(1);
    }
    #region  //turns
    void TurnGenerator()
    {
        float remainTime = timePlay;

        while (remainTime > 0)
        {
            int rnd = Random.Range(betweenTurnTime.x, betweenTurnTime.y + 1);

            if (remainTime >= rnd)
            {
                playerTurns1.Add(rnd);
                playerTurns2.Add(rnd);
                remainTime -= rnd;
            }
            else
            {
                int index = IndexOfSmallest(playerTurns1);
                playerTurns1[index] += remainTime;
                playerTurns2[index] += remainTime;
                remainTime = 0;
            }
        }
    // Invoke("startMatch",3);
    }
    
    void startMatch()
    {
        StartCoroutine(TurnMatch(0));
        
    }
    IEnumerator TurnMatch(int playerIdTurn)
    {
        if(playerIdTurn==0)
        {
            redPlayerSprite.SetActive(true);
            bluePlayerSprite.SetActive(false); 
        }
        else
        {
            redPlayerSprite.SetActive(false);
            bluePlayerSprite.SetActive(true); 
        }
 
        turnPlayer = playerIdTurn;

        List<float> currentList = playerIdTurn == 0 ? playerTurns1 : playerTurns2;

        if (currentList.Count == 0)
        {
            finish = true;
            Debug.Log("Game Finished");
            yield break;
        }

        int index = Random.Range(0, currentList.Count);
        float timeTurn = currentList[index];
        if(playerIdTurn == 0)
            playerTurns1.RemoveAt(index);
        else
            playerTurns2.RemoveAt(index);

        yield return new WaitForSeconds(timeTurn);

        StartCoroutine(TurnMatch(playerIdTurn == 0 ? 1 : 0));
    }
    int IndexOfSmallest(List<float> list)
    {
        int index = 0;
        float min = list[0];

        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < min)
            {
                min = list[i];
                index = i;
            }
        }
        return index;
    }
    #endregion
    float PlayerCarWhenSpawn1=0,PlayerCarWhenSpawn2=0;
    int carWaitForSpawn1=0,carWaitForSpawn2=0;
    IEnumerator waitForApawnCar()
    {
       int playerIdTurn=0;
       while(true)
       {
        if(carWaitForSpawn1>0)
        {
        if(PlayerCarWhenSpawn1+0.3f<Time.time)
        {
        carWaitForSpawn1--;
        InstanceCar(0);
        PlayerCarWhenSpawn1=Time.time;
        }
        }
        if(carWaitForSpawn2>0)
        {
        if(PlayerCarWhenSpawn2+0.3f<Time.time)
        {
        carWaitForSpawn2--;
        InstanceCar(1);
        PlayerCarWhenSpawn2=Time.time;
        }
        }
  
        yield return null;
       }
    }
    public void CarGenerate(int playerIdTurn)
    {
        if(playerIdTurn==0)
        {
            carWaitForSpawn1++;
        }
        else 
        {
            carWaitForSpawn2++;
        }
    }   
    void InstanceCar(int playerIdTurn)
    {
        GameObject g = Instantiate(carObj[Random.Range(0,carObj.Count)],playerIdTurn==0?playerSplines1[0].gameObject.transform.position:playerSplines2[0].gameObject.transform.position
        ,Quaternion.identity);
        CrossRoadCar c=g.GetComponent<CrossRoadCar>();
        c.setDefault(playerIdTurn);

        if(playerIdTurn==0)
        {
            playerCars1.Add(g);
            c.TakeMove(playerSplines1[0]);
            
        }
        else
        {
            playerCars2.Add(g);
            c.TakeMove(playerSplines2[0]);
        }
    }
    public void OnClicked(int playerId, int dir)
    {
        if (finish||turnPlayer!=playerId) return;
         CrossRoadCar activeCar;
        if(playerId==0)
        {
             activeCar = playerCars1[0].GetComponent<CrossRoadCar>();
        }
        else
        {
             activeCar = playerCars2[0].GetComponent<CrossRoadCar>();
        }
        if (!activeCar.readyToMove) return;
        dir+=activeCar.dir;
 
        if(dir>3)
        {
           dir=0; 
        }
        else if(dir<0)
        {
            dir=3;
        }
        
        if (dir == activeCar.direction)
        {
            if (playerId == 0) redPlayerScore++;
            else bluePlayerScore++;
        }
        else
        {
            if (playerId == 0) redPlayerScore--;
            else bluePlayerScore--;
        }
        bluePlayerTxt.text = bluePlayerScore.ToString();
        redPlayerTxt.text = redPlayerScore.ToString();
        if(dir==0)return;
        if(playerId==0)
        {
         
        playerCars1.RemoveAt(0);
        activeCar.TakeMove(playerSplines1[dir]);
        }
        else
        {
        playerCars2.RemoveAt(0);
        activeCar.TakeMove(playerSplines2[dir]);
        }
        activeCar.die();
        CarGenerate(turnPlayer);
    }

}
