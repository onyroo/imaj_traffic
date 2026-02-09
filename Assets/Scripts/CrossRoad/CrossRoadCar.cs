using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;
public class CrossRoadCar : MonoBehaviour
{
     [SerializeField] private float speed;
    [SerializeField] private GameObject rightLamp, leftLamp;
    public int dir = 0;
    public bool readyToMove = false;
    public int carId;
    public int direction = 0;

    public SplineAnimate anim;    
    [SerializeField] private Material mt;
    int cars=0;
    public void setDefault(int idNum)
    {
        carId = idNum;
        dir=carId;
        // Renderer r = GetComponent<Renderer>();
        // r.material.color = (carId == 0) ? Color.red : Color.blue;

        direction=0;
        readyToMove = false;
        leftLamp.SetActive(true);
        rightLamp.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SetDir")&&direction==0)
        {
            direction = Random.Range(1, 4);

            switch (direction)
            {
                case 1:
                    leftLamp.SetActive(false);
                    break;

                case 2:
                    leftLamp.SetActive(false);
                    rightLamp.SetActive(false);
                    break;

                case 3:
                    rightLamp.SetActive(false);
                    break;
                default:
                break;
            }
        }
        else if (other.CompareTag("SetReady"))
        {
            readyToMove = true;
        }
        else if (other.CompareTag("car"))
        {
            
           anim.Pause();
           anim.MaxSpeed = 0f;
           cars++;
        }
        else if (other.CompareTag("destroy"))
        {
            Destroy(gameObject);
        }
    }
    public void die()
    {
        Destroy(gameObject,5);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("car"))
        {
            
        cars--;
 
        if(cars==0)
            {
             anim.MaxSpeed = speed;
            anim.Play();
            }
        }
    }
    public void TakeMove(SplineContainer newContainer)
    {
        if(anim.NormalizedTime!=1&&anim.Container!=null)
        {
            StartCoroutine(nextAnim(newContainer));
            return;
        }
        anim.Container =newContainer;
        anim.NormalizedTime=0;
        anim.MaxSpeed = speed;
        anim.Play();
    }
    IEnumerator nextAnim(SplineContainer newContainer)
    {
        while(anim.NormalizedTime!=1)
        {
            yield return null;
        }
        TakeMove(newContainer);
    }
}
