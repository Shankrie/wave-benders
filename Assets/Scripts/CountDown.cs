using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class CountDown : MonoBehaviour
    {
        public GameObject[] CountDownObjectChilds;
        public GameObject waitingForPlayer;

        public KeyController SetKeyController { set { _mineKeyController = value; } }

        private KeyController _mineKeyController;
        private float _countDownTime = 0;
        private int _index = 0;

        private bool _startCountDown = false;

        // Use this for initialization
        void Start()
        {
            if (CountDownObjectChilds == null || CountDownObjectChilds.Length == 0)
            {
                throw new System.NotImplementedException("Count down must have children from which to begin countdown");
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_startCountDown && _countDownTime - 0.01 > Globals.Delays.COUNT_DOWN)
            {
                CountDownObjectChilds[_index++].SetActive(false);
                if (_index >= CountDownObjectChilds.Length)
                {
                    _startCountDown = false;
                    _mineKeyController.InitializeCall();
                }
                else
                {
                    CountDownObjectChilds[_index].SetActive(true);
                }
                _countDownTime = 0;
            }
            else if (_startCountDown)
            {
                _countDownTime += Time.fixedDeltaTime;
            }
        }

        public void StartCountDown()
        {
            waitingForPlayer.SetActive(false);
            _index = 0;
            _countDownTime = 0;
            _startCountDown = true;
            CountDownObjectChilds[_index].SetActive(true);
        }
    }
}
