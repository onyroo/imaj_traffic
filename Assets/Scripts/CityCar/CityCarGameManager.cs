using UnityEngine;

public class CityCarGameManager : MonoBehaviour
{
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Camera cam;

    [SerializeField] float minOrthoSize = 10f;
    [SerializeField] float maxOrthoSize = 100f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float edgePadding = 3f;

    void LateUpdate()
    {
        // if (player1 == null || player2 == null || cam == null) return;

        Vector3 center = (player1.position + player2.position) * 0.5f;
        Vector3 targetPos = new Vector3(center.x, cam.transform.position.y, center.z);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, moveSpeed * Time.deltaTime);

        Vector3 camPos = cam.transform.position;

        Vector3 v1 = cam.WorldToViewportPoint(player1.position);
        Vector3 v2 = cam.WorldToViewportPoint(player2.position);

        float minX = Mathf.Min(v1.x, v2.x);
        float maxX = Mathf.Max(v1.x, v2.x);
        float minY = Mathf.Min(v1.y, v2.y);
        float maxY = Mathf.Max(v1.y, v2.y);

        float zoomOut = 0f;
        float zoomIn = 0f;

        if (minX < 0.1f || maxX > 0.9f || minY < 0.1f || maxY > 0.9f)
            zoomOut = 1f;

        if (minX > 0.25f && maxX < 0.75f && minY > 0.25f && maxY < 0.75f)
            zoomIn = 1f;

        float targetSize = cam.orthographicSize;

        if (zoomOut > 0f)
            targetSize += zoomSpeed * Time.deltaTime * cam.orthographicSize;

        if (zoomIn > 0f)
            targetSize -= zoomSpeed * Time.deltaTime * cam.orthographicSize;

        targetSize = Mathf.Clamp(targetSize, minOrthoSize, maxOrthoSize);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}