using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntersectionGameManager : MonoBehaviour
{
    public static IntersectionGameManager Instance;

    [SerializeField] private Text bluePlayerTxt, redPlayerTxt;
    [SerializeField] private int bluePlayerScore, redPlayerScore;
    [SerializeField] private float timePlay;
    [SerializeField] private int turnCount;

    [SerializeField] private GameObject carObj;
    [SerializeField] private Transform startPoint;

    GameObject activeCar;
    int turnPlayer = 0;
    bool finish = false;

    void Awake()
    {
        StartCoroutine(TurnMatch(0, timePlay));
        Instance = this;
        CarGenerate(turnPlayer);
    }

    public void CarGenerate(int playerIdTurn)
    {
        activeCar = Instantiate(carObj, startPoint.position, startPoint.rotation);
        CarFindOneDir c = activeCar.GetComponent<CarFindOneDir>();
        c.setDefault(playerIdTurn);
    }

    public void OnClicked(int playerId, int dir)
    {
        if (finish) return;

        CarFindOneDir c = activeCar.GetComponent<CarFindOneDir>();
        if (c.carId != playerId) return;

        if (dir == c.direction && c.readyToMove)
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

    IEnumerator TurnMatch(int playerIdTurn, float timeTurn)
    {
        turnPlayer = playerIdTurn;

        yield return new WaitForSeconds(timeTurn);

        if (turnPlayer == 1)
        {
            if (turnCount <= 0)
            {
                finish = true;
                yield break;
            }

            turnCount--;
            StartCoroutine(TurnMatch(0, timePlay));
        }
        else
        {
            StartCoroutine(TurnMatch(1, timePlay));
        }
    }
}
