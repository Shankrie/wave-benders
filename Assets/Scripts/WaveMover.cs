using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMover : MonoBehaviour {

    //public float min = 5f;
    //public float max = 100f;
    //// Use this for initialization
    //void Start()
    //{

    //    min = transform.position.x;
    //    max = transform.position.x + 3;

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    transform.position = new Vector3(Mathf.PingPong(Time.time * 2, max - min) + min, transform.position.y, transform.position.z);
    //}
    public Transform Aposition;
    public Transform Bposition;
    private Vector3 pointB;
    private Vector3 pointA;
    private bool isFacingRight = true;

    IEnumerator Start()
    {
        //if (GlobalVariables.isHostOn && GlobalVariables.isClientOn)
        //{
            pointB = Bposition.position;
            pointA = Aposition.position;
            while (true)
            {
                yield return StartCoroutine(MoveObject(transform, pointA, pointB, 3.0f));
                yield return StartCoroutine(MoveObject(transform, pointB, pointA, 3.0f));
            }
        //}
    }

    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        var i = 0.0f;
        var rate = 1.0f / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            if (thisTransform.position == pointB || thisTransform.position == pointA)
                Flipper();
            yield return null;
        }
    }

    void Flipper()
    {
        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        // Flip collider over the x-axis
       // center.x = -center.x;
    }
}
