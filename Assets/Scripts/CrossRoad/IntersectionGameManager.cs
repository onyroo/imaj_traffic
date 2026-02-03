using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IntersectionGameManager : MonoBehaviour
{
    public static IntersectionGameManager Instance;

    [Header("UI")]
    [SerializeField] private Text bluePlayerTxt;
    [SerializeField] private Text redPlayerTxt;

    [Header("Score")]
    [SerializeField] private int bluePlayerScore;
    [SerializeField] private int redPlayerScore;

    [Header("Turn Settings")]
    [SerializeField] private float timePlay = 30f;
    [SerializeField] private Vector2Int betweenTurnTime = new Vector2Int(1, 3);

    [Header("Car")]
    [SerializeField] private GameObject carObj;
    [SerializeField] private Transform startPoint;

    private List<float> playerTurns1 = new List<float>();
    private List<float> playerTurns2 = new List<float>();

    private GameObject activeCar;
    private int turnPlayer;
    private bool finish;

    void Awake()
    {
        Instance = this;
        TurnGenerator();
        CarGenerate(0);
    }

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

        StartCoroutine(TurnMatch(0));
    }

    public void CarGenerate(int playerIdTurn)
    {
        if (activeCar != null)
            Destroy(activeCar);

        activeCar = Instantiate(carObj, startPoint.position, startPoint.rotation);
        activeCar.GetComponent<CarFindOneDir>().setDefault(playerIdTurn);
    }

    public void OnClicked(int playerId, int dir)
    {
        if (finish) return;

        CarFindOneDir c = activeCar.GetComponent<CarFindOneDir>();
        if (c.carId != playerId || !c.readyToMove) return;

        if (dir == c.direction)
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

        c.TakeMove(dir);
        CarGenerate(turnPlayer);
    }

    IEnumerator TurnMatch(int playerIdTurn)
    {
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
        currentList.RemoveAt(index);

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
}
