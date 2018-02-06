using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour {


    public GameObject[] CountDownObjectChilds;

    public Movement SetViewMovement { set { _mineViewMovement = value; } }
    private Movement _mineViewMovement;
    private float _countDownTime = 0;
    private int _index = 0;

    private bool _startCountDown = false;

	// Use this for initialization
	void Start () {

		if(CountDownObjectChilds == null || CountDownObjectChilds.Length == 0)
        {
            throw new System.NotImplementedException("Count down must have children from which to begin countdown");
        }
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        if(_startCountDown && _countDownTime - 0.01 > Globals.Delays.COUNT_DOWN)
        {
            CountDownObjectChilds[_index++].SetActive(false);
            if (_index >= CountDownObjectChilds.Length)
            {
                _startCountDown = false;
                _mineViewMovement.InitializeCall();
            }
            else
            {
                CountDownObjectChilds[_index].SetActive(true);
            }
            _countDownTime = 0;
        }
        else if(_startCountDown)
        {
            _countDownTime += Time.fixedDeltaTime;
        }
    }

    public void StartCountDown()
    {
        _index = 0;
        _countDownTime = 0;
        _startCountDown = true;
        CountDownObjectChilds[_index].SetActive(true);
    }
}
