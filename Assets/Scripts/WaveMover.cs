using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMover : MonoBehaviour {

    public Transform Aposition;
    public Transform Bposition;

    private Vector3 overflowPointA;
    private Vector3 overflowPointB;
    private Vector3 startPosition;

    private float maxEndXPoint;
    private float maxStartXPoint;
    private float speed = 1.0f;
    private float time;
    private float journeyLength;

    private bool isFacingRight = true;
    private bool notReachedEnd = true;
    private bool checkReachedEnd = false;

    const float INCREASE_SCALE_Y = 0.05f;
    const float INCREASE_SCALE_X = 0.025f;
    const float DECREASE_TIMER_VALUE = 0.1f;
    const float MAX_SCALE_Y = 1.0f;
    const float MIN_TIMER_VALUE = 1.0f;

    private KeyGenerator keyGen;

    

    void Start()
    {
        time = Time.time;
        startPosition = transform.position;
        transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
        overflowPointA = Aposition.position + new Vector3(-5, 0, 0);
        overflowPointB = Bposition.position + new Vector3(5, 0, 0);
        maxStartXPoint = Aposition.position.x;
        maxEndXPoint = Bposition.position.x;
        //StartCoroutine(MoveOverSeconds());
        journeyLength = Vector3.Distance(startPosition, overflowPointB);
        ParticleSystem ps = GameObject.FindGameObjectWithTag("ParticleSystem").GetComponent<ParticleSystem>();
        ps.GetComponent<Renderer>().sortingLayerName = "Foreground";
    }

    //public IEnumerator MoveOverSeconds()
    //{
    //    float elapsedTime = 0;
    //    while (true)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        transform.position = Vector3.Lerp(startPosition, overflowPointB, (elapsedTime / time) * speed);  
    //        yield return new WaitForEndOfFrame();
    //    }
    //    //transform.position = end;
    //}

    private void Update()
    {
        float dTime = (Time.time - time) * speed;
        float fracJourney = dTime / journeyLength;
        transform.position = Vector3.Lerp(startPosition, overflowPointB, fracJourney);
        checkOverflowPoints();

        if (GameObject.Find("KeyGen(Clone)") == null)
            return;

        keyGen = GameObject.Find("KeyGen(Clone)").GetComponent<KeyGenerator>();

        if (checkReachedEnd)
        {
            GameObject playerAvatar;
            if (keyGen.hostMove)
            {
                playerAvatar = GameObject.Find("Penguin");
                playerAvatar.GetComponent<LoseController>().playerHaveLost();
            }
            else
            {
                playerAvatar = GameObject.Find("Seal");
                playerAvatar.GetComponent<LoseController>().playerHaveLost();
            }

            return;
        }

        if (keyGen.deflectWave)
        {
            GameObject playerAvatar;
            if (keyGen.hostMove)
            {
                playerAvatar = GameObject.Find("Seal");
                playerAvatar.GetComponent<Animator>().SetTrigger("Clap");
            }
            else {
                playerAvatar = GameObject.Find("Penguin");
                playerAvatar.GetComponent<Animator>().SetTrigger("Flail");
            }



            if (!deflectWave())
            {
                if (keyGen.hostMove)
                {
                    playerAvatar = GameObject.Find("Seal");
                    playerAvatar.GetComponent<LoseController>().playerHaveLost();
                }
                else
                {
                    playerAvatar = GameObject.Find("Penguin");
                    playerAvatar.GetComponent<LoseController>().playerHaveLost();
                }
            }

            keyGen.deflectWave = false;
            if (keyGen.difficulty < 9)
            {
                keyGen.difficulty++;
            }
        }
    }

    void checkOverflowPoints()
    {
        if (checkReachedEnd == false && (transform.position.x <= maxStartXPoint || transform.position.x >= maxEndXPoint))
        {
            checkReachedEnd = true;
        }
        else if (checkReachedEnd == true && (Mathf.Abs(transform.position.x) >= Mathf.Abs(overflowPointB.x) - 0.1f))
        {
            notReachedEnd = false;
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
        overflowPointA += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
        overflowPointB += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
    }


    void PlayWaveRising()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("waveRising") as AudioClip;
        audioSource.Play();
    }

    public bool deflectWave()
    {
        if (checkReachedEnd == true)
        {
            return false;
        }

        // Recalulacting journey length with current position
        startPosition = transform.position;
        Vector3 temp = overflowPointA;
        overflowPointA = overflowPointB;
        overflowPointB = temp;
        journeyLength = Vector3.Distance(startPosition, overflowPointB);
        time = Time.time;

        // Fliping scaling object
        Flipper();
        ScaleAndLiftWave();
        PlayWaveRising();

        speed += 0.25f;

        return true;
    }


}
