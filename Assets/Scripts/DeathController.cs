using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathController : MonoBehaviour {

    private GameController _gameController;

    private Vector3 _startPoint;
    private Vector3 _endPoint;

    private float _distance;
    private float _time;
    private float _speed = 5f;

    private int _direction = 0;

    private bool _lost = false;

    private void Start()
    {
        // Get Game Controller to end game when player lost
        try {
            GameObject gameControllerObject = GameObject.FindGameObjectWithTag(Globals.Tags.GameController);
            _gameController = gameControllerObject.GetComponent<GameController>();
        }
        catch(Exception ex)
        {
            throw new Exception("Game Controller object couldn't be found. " + ex.ToString());
        }
    }

    public void playerHaveLost (int direction)
    {
        // set parameters to start death "animation".
        _lost = true;
        _direction = direction;
        _startPoint = transform.position;
        _endPoint = transform.position + new Vector3(3 * direction, 2, 0);
        _distance = Vector3.Distance(_startPoint, _endPoint);
        _time = Time.time;
        return;
    }

    private void Update()
    {
        if (_lost == true)
        {
            // move player to it's final destination
            float dTime = (Time.time - _time) * _speed;
            float fracDistance = dTime / _distance;
            int pozX = Mathf.RoundToInt(transform.position.x);
            transform.position = Vector3.Lerp(_startPoint, _endPoint, fracDistance);
            transform.Rotate(0, 0, 10 * Mathf.Sign(pozX));

            // Check if reached end point
            if(transform.position.y >= _endPoint.y - 0.1f)
            {
                _lost = false;
                _gameController.EndGame();
            }
        }
    }
}
