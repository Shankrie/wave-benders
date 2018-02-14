using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TAHL.WAVE_BENDER
{
    public class GameController : MonoBehaviour
    {
        // Initial Master Client Position
        public Transform MasterClientPosition;
        // Initial Client Position
        public Transform ClientPosition;

        private GameObject _masterClientObject;
        private GameObject _clientObject;
        private GameObject _gameEndGUI;
        private GameObject _networkGUI;

        private KeyController _masterClientKeyController;
        private KeyController _clientKeyController;


        private void Awake()
        {
            _masterClientObject = null;
            _clientObject = null;

            _gameEndGUI = GameObject.FindGameObjectWithTag(Globals.Tags.GameEndGUI);
            _gameEndGUI.SetActive(false);

            _networkGUI = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkGUI);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
        public void GetPlayerData()
        {
            try
            {
                // Calls to reset level on all clients
                if (_masterClientKeyController == null)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag(Globals.Tags.Player);
                    foreach (GameObject player in players)
                    {
                        if (player.GetComponent<KeyController>().PlayerDirection == 1)
                        {
                            _masterClientObject = player;
                            _masterClientKeyController = player.GetComponent<KeyController>();
                        }
                        else
                        {
                            _clientObject = player;
                            _clientKeyController = player.GetComponent<KeyController>();
                        }
                    }
                    if (_masterClientObject == null)
                    {
                        throw new System.Exception("Cannot find player who controls photon view");
                    }
                    else if (_clientObject == null)
                    {
                        throw new System.Exception("Cannot find player who doesn't control photon view");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        /// <summary>
        /// Called only from movement component
        /// </summary>
        public void ResetLevel()
        {
            _masterClientObject.transform.position = MasterClientPosition.position;
            _masterClientObject.transform.rotation = Quaternion.identity;

            _clientObject.transform.position = ClientPosition.position;
            _clientObject.transform.rotation = Quaternion.identity;

            _gameEndGUI.SetActive(false);
        }

        public void ReturnToLobby()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene((int)Globals.SceneIndex.Game);
        }
    }
}