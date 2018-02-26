using Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class NetworkManager : PunBehaviour
    {
        #region Public variables
        public ChatController ChatController = null;
        #endregion

        #region Private variables

        private enum NetworkButtonsEnum
        {
            Connect,
            Disconnect,
            Host,
            Join,
            StartGame,
            Return,
            Exit
        }

        private enum NetworkInputsEnum
        {
            PlayerName,
            RoomName,
            Region
        }

        private enum NetworkTextEnum
        {
            RequiredToConnect,
            CurrentlyConnected,
            NetworkState,
            ErrorState
        }

        private Button[] _networkButtons = null;
        private Selectable[] _networkInputs = null;
        private Text[] _networkTexts = null;

        private GameObject _chatGUI = null;

        private string[] _networkBtnNames = new string[] { "Connect", "Disconnect", "Host", "Join", "StartGame", "Return", "Exit" };
        private string[] _networkInputNames = new string[] { "PlayerName", "RoomName", "RegionDropdown" };
        private string[] _networkTextNames = new string[] { "RequiredToConnect", "CurrentlyConnected", "NetworkState", "ErrorState" };

        private string _networkState = string.Empty;
        private string _errorState = string.Empty;
        private string _playerName = string.Empty;
        private string _roomName = string.Empty;

        private const string LocalIp = "127.0.0.1";

        private bool _joinedRoom = false;
        private bool _isConnected = false;

        #endregion vars

        #region private methods

        private void Start()
        {
            if (ChatController == null)
                throw new Exception("Error. Chat controller not supplied in network manager script");

            SetupUI(ref _networkButtons, _networkBtnNames, Globals.Tags.NetworkButtons);
            SetupUI(ref _networkInputs, _networkInputNames, Globals.Tags.NetworkInputs);
            SetupUI(ref _networkTexts, _networkTextNames, Globals.Tags.NetworkTexts);

            foreach (Selectable networkInput in _networkInputs)
            {
                networkInput.interactable = false;
            }

            _networkButtons[(int)NetworkButtonsEnum.Disconnect].interactable = false;
            _networkButtons[(int)NetworkButtonsEnum.Host].interactable = false;
            _networkButtons[(int)NetworkButtonsEnum.Join].interactable = false;
            _networkButtons[(int)NetworkButtonsEnum.StartGame].interactable = false;

            _networkTexts[(int)NetworkTextEnum.RequiredToConnect].gameObject.SetActive(false);
            _networkTexts[(int)NetworkTextEnum.CurrentlyConnected].gameObject.SetActive(false);

            _chatGUI = GameObject.FindGameObjectWithTag(Globals.Tags.ChatGUI);
            if (_chatGUI == null)
                throw new Exception("Error. No Chat GUI is present or tag is missing");
            _chatGUI.SetActive(false);

            PhotonNetwork.sendRate = 30;
            PhotonNetwork.sendRateOnSerialize = 30;
            PhotonNetwork.automaticallySyncScene = true;
        }



        private void Update()
        {
            if (PhotonNetwork.connected != _isConnected)
            {
                _isConnected = PhotonNetwork.connected;
                foreach (Selectable networkInput in _networkInputs)
                {
                    networkInput.interactable = _isConnected;
                }

                _networkButtons[(int)NetworkButtonsEnum.Connect].interactable = !_isConnected;
                _networkButtons[(int)NetworkButtonsEnum.Disconnect].interactable = _isConnected;
                _networkButtons[(int)NetworkButtonsEnum.Host].interactable = _isConnected;
                _networkButtons[(int)NetworkButtonsEnum.Join].interactable = _isConnected;
                _networkButtons[(int)NetworkButtonsEnum.StartGame].interactable = false;
            }

            _networkTexts[(int)NetworkTextEnum.NetworkState].text = PhotonNetwork.connectionState.ToString() + ". " + _networkState;
            _networkTexts[(int)NetworkTextEnum.ErrorState].text = _errorState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="UIComponents">Array with fixed length</param>
        /// <param name="names">names for gameobject to check</param>
        /// <param name="tag">tag </param>
        private void SetupUI<T>(ref T[] UIComponents, string[] names, string tag)
        {
            GameObject[] UIObjects = GameObject.FindGameObjectsWithTag(tag);
            UIComponents = new T[UIObjects.Length];
            for (int i = 0; i < UIObjects.Length; i++)
            {
                UIComponents[i] = UIObjects[i].GetComponent<T>();
            }

            // Sort by names
            for (int i = 0; i < UIObjects.Length - 1; i++)
            {
                int j = i;
                while (UIObjects[i].name != names[i])
                {
                    j++;
                    if (UIObjects.Length <= j)
                    {
                        throw new Exception("Cannot find name " + names[i]);
                    }
                    Swap<GameObject>(ref UIObjects[i], ref UIObjects[j]);
                    Swap<T>(ref UIComponents[i], ref UIComponents[j]);
                }
            }
        }

        private void Swap<T>(ref T firstObject, ref T secondObject)
        {
            T temp = firstObject;
            firstObject = secondObject;
            secondObject = temp;

        }

        private bool IsReadyToPair()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                _errorState = "Connect first!";
                return false;
            }

            // Check if player name is entered
            _playerName = PlayerPrefs.GetString(Globals.PUNKeys.playerName);
            if (string.IsNullOrEmpty(_playerName))
            {
                _errorState = "Room name must be filled";
                return false;
            }

            // Assert that each player has unique name.
            if (!IsPlayerNameUnique())
                return false;

            // Check if room name is entered
            _roomName = PlayerPrefs.GetString(Globals.PUNKeys.gameRoomName);
            if (string.IsNullOrEmpty(_roomName))
            {
                _errorState = "Room name must be filled";
                return false;
            }
            return true;
        }

        private bool IsPlayerNameUnique()
        {
            _playerName = PlayerPrefs.GetString(Globals.PUNKeys.playerName);

            // Assert that each player has unique name.
            PhotonPlayer photonPlayer = PhotonNetwork.playerList.FirstOrDefault(
                player => player.NickName.ToLower() == _playerName.ToLower()
            );
            if (photonPlayer != null)
            {
                _errorState = String.Format("Name {0} is already taken!", _playerName);
                return false;
            }
            return true;
        }

        #endregion


        #region Photon inherited methods and events

        public override void OnJoinedLobby()
        {
            _errorState = string.Empty;
            _networkState = "Joined in lobby";
        }

        public override void OnJoinedRoom()
        {
            _chatGUI.SetActive(true);
            ChatController.InitializeChatConnection();

            _errorState = string.Empty;
            _networkState = "Joined room";

            foreach (Selectable networkInput in _networkInputs)
            {
                networkInput.interactable = false;
            }

            _networkButtons[(int)NetworkButtonsEnum.Host].interactable = false;
            _networkButtons[(int)NetworkButtonsEnum.Join].interactable = false;

            _networkTexts[(int)NetworkTextEnum.RequiredToConnect].gameObject.SetActive(true);
            _networkTexts[(int)NetworkTextEnum.CurrentlyConnected].text = "Currently Connected: " + PhotonNetwork.playerList.Length;
            _networkTexts[(int)NetworkTextEnum.CurrentlyConnected].gameObject.SetActive(true);

            if (PhotonNetwork.isMasterClient)
            {
                _networkButtons[(int)NetworkButtonsEnum.StartGame].interactable = true;
            }
            _joinedRoom = true;


        }

        #endregion


        #region Connect, Create or Join Rooms, Disconnect on click methods

        public void ConnectToServer()
        {
            if (PhotonNetwork.connected)
                return;

            _errorState = string.Empty;

            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.autoJoinLobby = true;

            int region = PlayerPrefs.GetInt(Globals.PUNKeys.cloudRegion);
            string regionName = (string)Enum.GetName(typeof(CloudRegionCode), region);
            Array values = Enum.GetValues(typeof(CloudRegionCode));
            PhotonNetwork.ConnectToRegion((CloudRegionCode)values.GetValue(region), "v1.0");

            // Use to connect to local server
            //PhotonNetwork.ConnectToMaster(LocalIp, Port, string.Empty, Globals.PUNVersion);
        }

        public void DisconnectFromServer()
        {
            _networkState = string.Empty;
            PhotonNetwork.Disconnect();
        }

        public void HostRoom()
        {
            _errorState = string.Empty;
            if (!IsReadyToPair())
                return;

            RoomOptions roomOpts = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(_roomName, roomOpts, TypedLobby.Default);
        }

        public void JoinRoom()
        {
            _errorState = string.Empty;
            if (!IsReadyToPair())
                return;

            // Check if specified room exists and if player with same name doesn't exist
            bool roomExist = PhotonNetwork.GetRoomList().Count(room => room.Name == _roomName) > 0;
            if (roomExist)
                PhotonNetwork.JoinRoom(_roomName);
            else
                _errorState = "Room doesn't exist";
        }

        public void StartGame()
        {
            if (PhotonNetwork.playerList.Length > 1)
            {
                PhotonNetwork.LoadLevel((int)Globals.SceneIndex.Game);
            }
            else
            {
                _errorState = "Wait for opponnent to connect";
            }
        }

        public void Return()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene((int)Globals.SceneIndex.MainMenu);
        }

        public void Exit()
        {
            PhotonNetwork.Disconnect();
            Application.Quit();
        }

        #endregion connect methods
    }
}