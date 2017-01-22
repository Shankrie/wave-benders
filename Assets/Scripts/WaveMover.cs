using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMover : MonoBehaviour {

    public Transform Aposition;
    public Transform Bposition;

    private Vector3 overflowPointA;
    private Vector3 overflowPointB;

    private float maxEndXPoint;
    private float maxStartXPoint;
    private float speed = 5.0f;
    private float time = 10f;

    private bool isFacingRight = true;
    private bool notReachedEnd = true;
    private bool checkReachedEnd = false;

    const float INCREASE_SCALE_Y = 0.05f;
    const float INCREASE_SCALE_X = 0.025f;
    const float DECREASE_TIMER_VALUE = 0.1f;
    const float MAX_SCALE_Y = 1.0f;
    const float MIN_TIMER_VALUE = 1.0f;
    

    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
        overflowPointA = Aposition.position + new Vector3(-5, 0, 0);
        overflowPointB = Bposition.position + new Vector3(5, 0, 0);
        maxStartXPoint = Aposition.position.x;
        maxEndXPoint = Bposition.position.x;
        MoveOverSpeed();
        StartCoroutine(MoveOverSeconds());
    }

    public IEnumerator MoveOverSpeed()
    {
        // speed should be 1 unit per second
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, overflowPointB, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveOverSeconds()
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(transform.position, overflowPointB, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //transform.position = end;
    }

    //void Update()
    //{
    //    if (checkReachedEnd == false && (transform.position.x <= maxStartXPoint || transform.position.x >= maxEndXPoint))
    //    {
    //        checkReachedEnd = true;
    //    }
    //    else if (checkReachedEnd == true && (Mathf.Abs(transform.position.x) >= Mathf.Abs(overflowPointB.x) - 0.1f))
    //    {
    //        notReachedEnd = false;
    //    }



    //    //if (Input.GetKeyDown(KeyCode.A))
    //    //{
    //    //    Flipper();
    //    //    //ScaleAndLiftWave();
    //    //    //SpeedIncreaser();
    //    //    //PlayWaveRising();
    //    //    GlobalVariables.sendWaveAway = false;
    //    //    Vector3 temp = overflowPointA;
    //    //    overflowPointA = overflowPointB;
    //    //    overflowPointB = temp;
    //    //    yield return null;
    //    //}


    //}
   
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
        overflowPointA += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
        overflowPointB += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
    }


    void PlayWaveRising()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("waveRising") as AudioClip;
        audioSource.Play();
    }

    public void deflectWave()
    {
        Flipper();
        ScaleAndLiftWave();
        PlayWaveRising();
        GlobalVariables.sendWaveAway = false;
        Vector3 temp = overflowPointA;
        overflowPointA = overflowPointB;
        overflowPointB = temp;
    }

}
