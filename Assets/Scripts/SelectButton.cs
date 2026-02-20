using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour
{
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
