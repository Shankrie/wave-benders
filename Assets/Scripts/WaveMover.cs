using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class WaveMover : MonoBehaviour
    {
        public Transform Aposition;
        public Transform Bposition;

        private Vector3 _overflowPointA;
        private Vector3 _overflowPointB;
        private Vector3 _startPosition;
        private Vector3 _initialScale;
        private Vector3 _initialPosition;

        private float _speed = Globals.Defaults.WaveSpeed;
        private float _lastSpeed = Globals.Defaults.WaveSpeed;
        private float _time;
        private float _journeyLength;

        private bool _isFacingRight = true;
        private bool _reachedEnd = false;

        const float INCREASE_SCALE_Y = 0.05f;
        const float INCREASE_SCALE_X = 0.025f;
        const float INCREASE_SPEED = 0.25f;
        const float MAX_SCALE_Y = 1.0f;
        const float MAX_SPEED = 5.0f;

        void Start()
        {
            PlayWaveRising(true);

            transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);

            _time = Time.time;
            _startPosition = transform.position;
            _overflowPointA = Aposition.position;
            _overflowPointB = Bposition.position;

            _initialScale = transform.localScale;
            _initialPosition = transform.position;

            _journeyLength = Vector3.Distance(_startPosition, _overflowPointB);
            _isFacingRight = true;
        }

        private void Update()
        {
            if (_reachedEnd)
                return;

            float d_time = (Time.time - _time) * _speed;
            float fracJourney = d_time / _journeyLength;
            transform.position = Vector3.Lerp(_startPosition, _overflowPointB, fracJourney);

            // Wave reached it's overflow point which means game end
            if (transform.position.x > Mathf.Abs(_overflowPointB.x) - 0.1f)
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
            if(_isFacingRight) {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            _isFacingRight = !_isFacingRight;
        }

        public void IncreaseSpeed()
        {
            _speed = Mathf.Clamp(_speed + INCREASE_SPEED * 2, _lastSpeed, MAX_SPEED);
            _time = Time.time;
            _startPosition = transform.position;
            _journeyLength = Vector3.Distance(_startPosition, _overflowPointB);
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

        public bool DeflectWave()
        {
            // Recalulacting journey length with current position
            _startPosition = transform.position;

            // flip wave destination
            Vector3 temp = _overflowPointA;
            _overflowPointA = _overflowPointB;
            _overflowPointB = temp;

            // recalculate wave journey distance
            _journeyLength = Vector3.Distance(_startPosition, _overflowPointB);
            _time = Time.time;

            // Fliping scaling object
            Flipper();
            ScaleAndLiftWave();

            // Increase speed
            _speed = Mathf.Clamp(_speed + INCREASE_SPEED, _lastSpeed, MAX_SPEED);
            _lastSpeed = _speed;

            return true;
        }

        public void ResetWave()
        {
            if (!_isFacingRight)
            {
                Flipper();
            }
            // Setup positions and scale
            transform.localScale = _initialScale;
            transform.position = _initialPosition;
            _startPosition = _initialPosition;

            _overflowPointA = Aposition.position;
            _overflowPointB = Bposition.position;

            _speed = Globals.Defaults.WaveSpeed;

            _reachedEnd = false;
            enabled = false;

        }
    }
}