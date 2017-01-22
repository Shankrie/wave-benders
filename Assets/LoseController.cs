using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseController : MonoBehaviour {

    public Transform endPoint;

    private Vector3 startPoint;

    private float distance;
    private float time;
    private float speed = 5f;

    private bool lost = false;
    


    public void playerHaveLost ()
    {
        lost = true;
        startPoint = transform.position;
        distance = Vector3.Distance(startPoint, endPoint.position);
        time = Time.time;
        return;
    }

    private void Update()
    {
        if (lost == true)
        {
            float dTime = (Time.time - time) * speed;
            float fracDistance = dTime / distance;
            int pozX = Mathf.RoundToInt(transform.position.x);
            transform.position = Vector3.Lerp(startPoint, endPoint.position, fracDistance);
            transform.Rotate(0, 0, 10 * Mathf.Sign(pozX));

            if(transform.position.y >= endPoint.position.y - 0.1f)
            {
                Destroy(this);
            }
        }
    }
}
