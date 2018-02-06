using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    // Initial Master Client Position
    public Transform MasterClientPosition;
    // Initial Client Position
    public Transform ClientPosition;


    private GameObject _masterClientObject;
    private GameObject _clientObject;
    private GameObject _waveObject;
    private GameObject _gameEndGUI;
    private GameObject _networkGUI;

    private WaveMover _waveMover;

    private void Awake()
    {
        _masterClientObject = null;
        _clientObject = null;
        _waveObject = null;
        _waveMover = null;
        _gameEndGUI = GameObject.FindGameObjectWithTag(Globals.Tags.GameEndGUI);
        _gameEndGUI.SetActive(false);

        _networkGUI = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkGUI);
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            _gameEndGUI.SetActive(!_gameEndGUI.activeSelf);
        }
	}

    public void EndGame()
    {
        _gameEndGUI.SetActive(true);
    }

    public void GotoMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene((int)Globals.SceneIndex.MainMenu);
    }

    /// <summary>
    /// Called on click, it's purpose is to call rpc which objective is to restart level
    /// </summary>
    public void RestartLevel()
    {
        try
        {
            // Calls to reset level on all clients
            if (_masterClientObject == null)
            {
                _masterClientObject = GameObject.FindGameObjectWithTag(Globals.Tags.MasterClient);
            }
            _masterClientObject.GetComponent<Movement>().ResetLevelCall();
        }
        catch(Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    /// <summary>
    /// Called only from movement component
    /// </summary>
    public void ResetLevel()
    {
        try
        {
            if (_clientObject == null)
            {
                _clientObject = GameObject.FindGameObjectWithTag(Globals.Tags.Player);
            }
            if (_waveObject == null)
            {
                _waveObject = GameObject.FindGameObjectWithTag(Globals.Tags.Wave);
                _waveMover = _waveObject.GetComponent<WaveMover>();
            }

            _clientObject.transform.position = ClientPosition.position;
            _clientObject.transform.rotation = Quaternion.identity;

            _masterClientObject.transform.position = MasterClientPosition.position;
            _masterClientObject.transform.rotation = Quaternion.identity;

            // Reset wave position scale and facing direction
            _waveMover.ResetWave();
            _waveObject.SetActive(false);

            _gameEndGUI.SetActive(false);
        }
        catch(Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene((int)Globals.SceneIndex.Game);
    }


}
