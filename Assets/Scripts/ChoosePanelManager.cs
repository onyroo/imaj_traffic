using UnityEngine;

public class ChoosePanelManager : MonoBehaviour
{
    public void ChangeScene(int sceneIndex)
    {
     PlayerJoinManager.Instance.ChangeScene(sceneIndex);
    }
}
