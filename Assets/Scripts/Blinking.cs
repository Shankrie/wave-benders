using UnityEngine;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class Blinking : MonoBehaviour
    {
        private Text _text;

        private float _delay = 0.25f;
        private float _lastTime;

        private bool _enabled = true;

        // Use this for initialization
        void Start()
        {
            _text = GetComponent<Text>();
            _lastTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if(_lastTime < Time.time)
            {
                _enabled = !_enabled;
                _text.enabled = _enabled;
                _lastTime = Time.time + 0.25f;
            }
        }
    }
}