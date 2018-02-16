using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class GameController : MonoBehaviour
    {
        // Initial Master Client Position
        public Transform MasterClientPosition;
        // Initial Client Position
        public Transform ClientPosition;
        // Winner label in Game End GUI
        public Text WinnerLabel;
        public Text LoseCause;

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

        public void EnableGameEndGUI(bool enable)
        {
            _gameEndGUI.SetActive(enable);
        }

        public void SetWinner(bool win, string cause)
        {
            string message = "Winner";
            if (!win)
            {
                message = "Loser";
            }
            WinnerLabel.text = message;
            LoseCause.text = cause;
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
                KeyController mineKeyController = null;
                GameObject[] players = GameObject.FindGameObjectsWithTag(Globals.Tags.Player);
                foreach (GameObject player in players)
                {
                    KeyController keyController = player.GetComponent<KeyController>();
                    if(mineKeyController == null && keyController.ControlsView)
                    {
                        mineKeyController = keyController;
                    }
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
                mineKeyController.ResetLevelCall();
            }
            catch (Exception ex)
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
}