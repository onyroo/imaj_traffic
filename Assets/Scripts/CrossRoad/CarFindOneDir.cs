using UnityEngine;
using System.Collections;

public class CarFindOneDir : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed;
    [SerializeField] private GameObject rightLamp, leftLamp;

    public bool readyToMove = false;
    public int carId;
    public int direction = 0;

    [SerializeField] private Material mt;

    public void setDefault(int idNum)
    {
        GameObject parent = GameObject.Find("points");
        if (parent != null)
        {
            int count = parent.transform.childCount;
            points = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                points[i] = parent.transform.GetChild(i);
            }
        }
        carId = idNum;
        Renderer r = GetComponent<Renderer>();
        r.material.color = (carId == 0) ? Color.red : Color.blue;


        readyToMove = false;

        if (leftLamp) leftLamp.SetActive(true);
        if (rightLamp) rightLamp.SetActive(true);

        StartCoroutine(AnimationMove(points[0]));
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SetDir"))
        {
            direction = Random.Range(1, 4);

            switch (direction)
            {
                case 1:
                    if (leftLamp) leftLamp.SetActive(false);
                    break;

                case 2:
                    if (leftLamp) leftLamp.SetActive(false);
                    if (rightLamp) rightLamp.SetActive(false);
                    break;

                case 3:
                    if (rightLamp) rightLamp.SetActive(false);
                    break;
            }
        }
        else if (other.CompareTag("SetReady"))
        {
            readyToMove = true;
        }
    }

    public void TakeMove(int dir)
    {
        StopAllCoroutines();

        switch (dir)
        {
            case 1:
                StartCoroutine(AnimationMove(points[1]));
                break;
            case 2:
                StartCoroutine(AnimationMove(points[2]));
                break;
            case 3:
                StartCoroutine(AnimationMove(points[3]));
                break;
        }

        Destroy(gameObject, 2f);
    }

    IEnumerator AnimationMove(Transform target)
    {
        Vector3 startPos=transform.position;
        float t=0;
        while (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                startPos,
                target.position,
                t
            );
            t+=speed * Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
    }
}
