using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ChoosePanelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> markers = new();

    private void Start()
    {
        for (int i = 0; i < markers.Count; i++)
        {
            if (PlayerPrefs.GetInt("level" + i.ToString(), 0) == 1)
            {
                markers[i].SetActive(true);
            }
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        if (PlayerPrefs.GetInt("level" + (sceneIndex - 2).ToString(), 0) == 1)
            return;

        PlayerPrefs.SetInt("level" + (sceneIndex - 2).ToString(), 1);
        PlayerPrefs.Save();

        PlayerJoinManager.Instance.ChangeScene(sceneIndex);
    }
}