using UnityEngine;

public class DestroyWrongCar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("car"))
        {
        plathformerManager.Instance._removeCar(other.gameObject);
        }
    }
}
