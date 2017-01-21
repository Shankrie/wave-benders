using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMover : MonoBehaviour {
    public Transform WaveParent;

    public Transform Aposition;
    public Transform Bposition;

    private Vector3 pointB;
    private Vector3 pointA;

    private float timer = 4.0f;

    private bool isFacingRight = true;

    const float INCREASE_SCALE_Y = 0.05f;
    const float INCREASE_SCALE_X = 0.025f;
    const float DECREASE_TIMER_VALUE = 0.1f;
    const float MAX_SCALE_Y = 1.0f;
    const float MIN_TIMER_VALUE = 1.0f;

    IEnumerator Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
        pointA = Aposition.position;
        pointB = Bposition.position;
        Vector3 temp;
        while (true)    
        {
            yield return StartCoroutine(MoveObject(transform, timer));
            temp = pointA;
            pointA = pointB;
            pointB = temp;
            yield return StartCoroutine(MoveObject(transform, timer));
            temp = pointA;
            pointA = pointB;
            pointB = temp;
        }    
    }
    
    void FixedUpdate()
    {
        
    }

    IEnumerator MoveObject(Transform thisTransform, float time)
    {
        var i = 0.0f;
        var rate = 1.0f / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
                
            if (thisTransform.position == pointB || thisTransform.position == pointA)
            {
                Flipper();
                ScaleAndLiftWave();
                SpeedIncreaser();
            }
            thisTransform.position = Vector3.Lerp(pointA, pointB, i);
            yield return null;
        }
    }

    void Flipper()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void ScaleAndLiftWave()
    {
        if (transform.localScale.y >= MAX_SCALE_Y)
            return;

        transform.localScale += new Vector3(INCREASE_SCALE_X * Mathf.Sign(transform.localScale.x), INCREASE_SCALE_Y, 0);
        pointA += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
        pointB += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
    }

    void SpeedIncreaser()
    {
        if(timer >= MIN_TIMER_VALUE)
        {
            timer -= DECREASE_TIMER_VALUE;
        }
    }

}
