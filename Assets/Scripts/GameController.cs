using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon;

namespace TAHL.WAVE_BENDER
{
    public class GameController : PunBehaviour
    {
        public GameObject MasterClient = null;

        public GameObject Client = null;
        // Winner label in Game End GUI
        public Text WinnerLabel;
        public Text LoseCause;
        public Text Error;

        private KeyController _mineKeyController = null;
        private GameObject[] _countDownObjs;

        private GameObject _gameEndGUI;
        private const string LocalIp = "127.0.0.1";

        private float _readyTime = -1;
        private float _countDownTime = 0;
        private int _index = 1;

        private bool _startCountDown = false;
        private bool _ready = true;

        private void Awake()
        {
            if (MasterClient == null)
            {
                throw new Exception("Error. Master client not supplied");
            }
            if (Client == null)
            {
                throw new Exception("Error. Client not supplied");
            }

            // Get Countdown objects and disable em
            Globals.Methods.SetupGOs(ref _countDownObjs, Globals.Names.CountDowns, Globals.Tags.CountDownObjs);
            foreach(GameObject go in _countDownObjs)
            {
                go.SetActive(false);
            }
            _countDownObjs[1].SetActive(true);

            if (PhotonNetwork.isMasterClient)
            {
                MasterClient.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                _mineKeyController = MasterClient.GetComponent<KeyController>();
            }
            else
            {
                Client.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                _mineKeyController = Client.GetComponent<KeyController>();
            }

            _gameEndGUI = GameObject.FindGameObjectWithTag(Globals.Tags.GameEndGUI);
            _gameEndGUI.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _gameEndGUI.SetActive(!_gameEndGUI.activeSelf);
            }

            // Get player ready
            if (!_ready)
            {
                if (_readyTime > 0)
                {
                    _readyTime -= Time.deltaTime;
                    return;
                }
                _countDownObjs[(int)Globals.Enums.CountDownTexts.GetReady].SetActive(false);
                _countDownObjs[(int)Globals.Enums.CountDownTexts.Waiting].SetActive(false);
                _countDownObjs[(int)Globals.Enums.CountDownTexts.Three].SetActive(true);
                _index = (int)Globals.Enums.CountDownTexts.Three;
                _ready = true;
            }

            if (_startCountDown && _countDownTime - 0.01 > Globals.Delays.COUNT_DOWN)
            {
                _countDownObjs[_index++].SetActive(false);
                if (_index >= _countDownObjs.Length)
                {
                    _startCountDown = false;
                    _mineKeyController.InitializeCall();
                }
                else
                {
                    _countDownObjs[_index].SetActive(true);
                }
                _countDownTime = 0;
            }
            else if (_startCountDown)
            {
                _countDownTime += Time.fixedDeltaTime;
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
            SceneManager.LoadScene((int)Globals.Enums.SceneIndex.MainMenu);
        }

        /// <summary>
        /// Called on click, it's purpose is to call rpc which objective is to restart level
        /// </summary>
        public void RestartLevel()
        {
            if (PhotonNetwork.playerList.Length == 0)
            {
                Error.text = "Error: Other player has left the game";
                return;
            }

            foreach (GameObject go in _countDownObjs)
            {
                go.SetActive(false);
            }
            _startCountDown = false;

            if (_mineKeyController == null)
            {
                throw new Exception("Error. Mine key controller is not defined");
            }
            _mineKeyController.ResetLevelCall();
        }

        public void ReturnToLobby()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene((int)Globals.Enums.SceneIndex.Lobby);
        }

        public void StartCountDown(bool restarting = false)
        {
            _index = 1;
            _countDownTime = 0f;
            _startCountDown = true;


            if (restarting)
            {
                _countDownObjs[0].SetActive(true);
                _readyTime = 3f;
                _ready = false;
            }
            else
            {
                _ready = true;
                _countDownObjs[_index].SetActive(true);
            }
        }

        public override void OnOwnershipTransfered(object[] viewAndPlayers)
        {
            Client.SetActive(true);
            MasterClient.SetActive(true);
        }

        private void OnApplicationQuit()
        {
            PhotonNetwork.Disconnect();
        }
    }
}