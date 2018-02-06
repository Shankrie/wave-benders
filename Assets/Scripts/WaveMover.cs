using UnityEngine;

public class WaveMover : MonoBehaviour {

    public Transform Aposition;
    public Transform Bposition;

    private Vector3 _overflowPointA;
    private Vector3 _overflowPointB;
    private Vector3 _startPosition;
    private Vector3 _initialScale;
    private Vector3 _initialPosition;

    private float _maxEndXPoint;
    private float _maxStartXPoint;
    private float _speed = Globals.Defaults.WaveSpeed;
    private float _lastSpeed = Globals.Defaults.WaveSpeed;
    private float _time;
    private float _journeyLength;

    private bool _isFacingRight = true;
    private bool _reachedEnd = false;

    const float INCREASE_SCALE_Y = 0.05f;
    const float INCREASE_SCALE_X = 0.025f;
    const float DECREASE__timeR_VALUE = 0.1f;
    const float MAX_SCALE_Y = 1.0f;
    const float MIN__timeR_VALUE = 1.0f;

    void Start()
    {
        PlayWaveRising(true);

        transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);

        _time = Time.time;
        _startPosition = transform.position;
        _overflowPointA = Aposition.position + new Vector3(-5, 0, 0);
        _overflowPointB = Bposition.position + new Vector3(5, 0, 0);
        _initialScale = transform.localScale;
        _initialPosition = transform.position;
        _maxStartXPoint = Aposition.position.x;
        _maxEndXPoint = Bposition.position.x;
        _journeyLength = Vector3.Distance(_startPosition, _overflowPointB);   
    }

    private void Update()
    {
        if (_reachedEnd)
            return;

        float d_time = (Time.time - _time) * _speed;
        float fracJourney = d_time / _journeyLength;
        transform.position = Vector3.Lerp(_startPosition, _overflowPointB, fracJourney);

        // Wave reached it's overflow point which means game end
        if (transform.position.x > _overflowPointB.x - 0.1f ||
            transform.position.x < _overflowPointA.x + 0.1f)
        {
            StopPlayWaveRising();
            _reachedEnd = true;
        }
    }

    private void OnEnable()
    {
        _time = Time.time;
    }

    public void Flipper()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void IncreaseSpeed()
    {
        _speed += Globals.Defaults.WaveSpeed;
    }

    void ScaleAndLiftWave()
    {
        if (transform.localScale.y >= MAX_SCALE_Y)
            return;

        transform.localScale += new Vector3(INCREASE_SCALE_X * Mathf.Sign(transform.localScale.x), INCREASE_SCALE_Y, 0);
        _overflowPointA += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
        _overflowPointB += new Vector3(0, INCREASE_SCALE_Y * 5, 0);
    }


    void PlayWaveRising(bool looper)
    {
        if (looper == false) return;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("waveRising") as AudioClip;
        audioSource.Play();
        audioSource.loop = looper;
    }

    void StopPlayWaveRising()
    {
        AudioSource audioSrc = GetComponent<AudioSource>();
        audioSrc.enabled = false;
    }

    void PlaySealBark()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("sealBarking") as AudioClip;
        audioSource.Play();
    }
    void PlayPenguinBattleCry()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("penguinBattleCry") as AudioClip;
        audioSource.Play();
    }

    public bool deflectWave()
    {
        // Recalulacting journey length with current position
        _startPosition = transform.position;
        Vector3 temp = _overflowPointA;
        _overflowPointA = _overflowPointB;
        _overflowPointB = temp;
        _journeyLength = Vector3.Distance(_startPosition, _overflowPointB);
        _time = Time.time;

        // Fliping scaling object
        Flipper();
        ScaleAndLiftWave();
        _speed = _lastSpeed + 0.25f;
        _lastSpeed = _speed;

        return true;
    }

    public void ResetWave()
    {
        if(!_isFacingRight)
        {
            Flipper();
        }
        // Setup positions and scale
        transform.localScale = _initialScale;
        transform.position = _initialPosition;
        _startPosition = _initialPosition;

        _overflowPointA = Aposition.position + new Vector3(-5, 0, 0);
        _overflowPointB = Bposition.position + new Vector3(5, 0, 0);

        _maxStartXPoint = Aposition.position.x;
        _maxEndXPoint = Bposition.position.x;

        _reachedEnd = false;
        enabled = false;
    }
}
