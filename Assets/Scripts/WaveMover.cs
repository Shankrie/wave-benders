using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMover : MonoBehaviour {

    public Transform Aposition;
    public Transform Bposition;
    private Vector3 pointB;
    private Vector3 pointA;
    private bool isFacingRight = true;
    //public float min = 0;
    //public float max = 0;

    //// Use this for initialization
    //void Start()
    //{

    //    min = Aposition.position.x;
    //    max = Bposition.position.x;

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //Debug.Log(GlobalVariables.isHostOn);
    //    //Debug.Log(GlobalVariables.isClientOn);
    //    //if (GlobalVariables.isHostOn && GlobalVariables.isClientOn)
    //    //{
    //        transform.position = new Vector3(Mathf.PingPong(Time.time*2, max - min) + min, transform.position.y,transform.position.z);
    //        if (transform.position == Bposition.position || transform.position == Aposition.position) Flipper();
    //    //}
    //}


    IEnumerator Start()
    {
        pointB = Bposition.position;
        pointA = Aposition.position;
        while (true)
        {
            yield return StartCoroutine(MoveObject(transform, pointA, pointB, 3.0f));
            yield return StartCoroutine(MoveObject(transform, pointB, pointA, 3.0f));
        }

    }

    void Update()
    {
        //Debug.Log(GlobalVariables.playerCount);
    }

    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        var i = 0.0f;
        var rate = 1.0f / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            if (thisTransform.position == pointB || thisTransform.position == pointA) Flipper();
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
