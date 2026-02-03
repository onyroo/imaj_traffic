using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ParkingGameManager : MonoBehaviour
{
    public static ParkingGameManager Instance { get; private set; }

    public int playerIdTurn = 0;
    public bool moving;

    [SerializeField] private float turnTime = 10f;
    [SerializeField] private Text timerTxt;

    private float timer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        StartCoroutine(TurnManager());
    }

    IEnumerator TurnManager()
    {
        while (true)
        {
            timer = turnTime;
 
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                if (timerTxt)
                    timerTxt.text = timer.ToString("F1");  

                yield return null;
            }
 
             ChangeTurn();

            // Debug.Log("Turn changed to player: " + playerIdTurn);
        }
    }
    public void setMove()
    {
        moving=true;
        StopAllCoroutines();
        timerTxt.text ="On Move";

    }
    public void setTurn()
    {
        moving=false;
        ChangeTurn();
        StartCoroutine(TurnManager());
    }
    void ChangeTurn()
    {
        playerIdTurn = (playerIdTurn == 0) ? 1 : 0;
    }
}
