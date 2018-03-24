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

        private KeyController _mineKeyController = null;

        private GameObject[] _countDownObjs;

        // Winner label in Game End GUI
        public Text WinnerLabel;
        public Text LoseCause;

        private GameObject _gameEndGUI;
        private GameObject _networkGUI;
        private const string LocalIp = "127.0.0.1";


        private float _countDownTime = 0;
        private int _index = 0;

        private bool _startCountDown = false;

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

            if (PhotonNetwork.isMasterClient)
            {
                MasterClient.SetActive(true);
                MasterClient.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                _mineKeyController = MasterClient.GetComponent<KeyController>();
            }
            else
            {
                MasterClient.SetActive(true);
                Client.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
                _mineKeyController = Client.GetComponent<KeyController>();
            }

            _gameEndGUI = GameObject.FindGameObjectWithTag(Globals.Tags.GameEndGUI);
            _gameEndGUI.SetActive(false);

            _networkGUI = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkGUI);

            if(Globals.NetworkData.Offline_Mode)
                OnOwnershipTransfered(null);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _gameEndGUI.SetActive(!_gameEndGUI.activeSelf);
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

        private void OnGUI()
        {
            // GUI.TextArea(new Rect(0, 0, 100, 100), _mineKeyController.gameObject.name);
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
            try
            {
                if (_mineKeyController == null)
                {
                    throw new Exception("Error. Mine key controller is not defined");
                }
                _mineKeyController.ResetLevelCall();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public void ReturnToLobby()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene((int)Globals.Enums.SceneIndex.Lobby);
        }

        public void StartCountDown()
        {
            _countDownObjs[0].SetActive(false);
            _index = 0;
            _countDownTime = 0;
            _startCountDown = true;
            _countDownObjs[_index].SetActive(true);
        }

        public override void OnOwnershipTransfered(object[] viewAndPlayers)
        {
            if(PhotonNetwork.isMasterClient)
            {
                Client.SetActive(true);
                _mineKeyController.StartCountDownCall();
            }
            else
            {
                MasterClient.SetActive(true);
            }
        }

        private void OnApplicationQuit()
        {
            PhotonNetwork.Disconnect();
        }
    }
}