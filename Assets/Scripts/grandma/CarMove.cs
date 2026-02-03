using UnityEngine;

public class CarMove : MonoBehaviour
{
    [SerializeField] private Vector2 limitMoveSpeed;
    float moveSpeed=0,targetSpeed=0;
    private void OnEnable() {
        moveSpeed=Random.Range(limitMoveSpeed.x,limitMoveSpeed.y+1);
        targetSpeed=moveSpeed;
    }
    void Update()
    {
        moveSpeed= Mathf.Lerp(moveSpeed,targetSpeed,Time.deltaTime*4);
        transform.position=transform.position + (moveSpeed*transform.right*Time.deltaTime);  
    // transform.position=transform.position + ((targetSpeed)*transform.right*Time.deltaTime);  
    }
    int c=0;
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("SafeWay")||other.CompareTag("car"))
        {
            targetSpeed=3;
            c++;
        }
         
    }

    
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("SafeWay")||other.CompareTag("car"))
        {
            c--;
            if(c==0)
            {
            targetSpeed=12;
            }
 
        }
    }
}
