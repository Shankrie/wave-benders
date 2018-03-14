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

        private Button[] _networkButtons = null;
        private Selectable[] _networkInputs = null;
        private Text[] _networkTexts = null;


        private string _networkState = string.Empty;
        private string _errorState = string.Empty;
        private string _playerName = string.Empty;
        private string _roomName = string.Empty;

        private const string LocalIp = "127.0.0.1";

        private bool _isConnected = false;

        #endregion vars

        #region private methods

        private void Start()
        {
            if (ChatController == null)
                throw new Exception("Error. Chat controller not supplied in network manager script");

            SetupUI(ref _networkButtons, Globals.Names.NetworkButtons, Globals.Tags.NetworkButtons);
            SetupUI(ref _networkInputs, Globals.Names.NetworkInputs, Globals.Tags.NetworkInputs);
            SetupUI(ref _networkTexts, Globals.Names.NetworkTexts, Globals.Tags.NetworkTexts);

            _networkInputs[(int)Globals.Enums.NetworkInputs.PlayerName].interactable = false;
            _networkInputs[(int)Globals.Enums.NetworkInputs.RoomName].interactable = false;
            _networkInputs[(int)Globals.Enums.NetworkInputs.CloudRegion].interactable = true;
            _networkInputs[(int)Globals.Enums.NetworkInputs.ChatRegion].interactable = true;

            _networkButtons[(int)Globals.Enums.NetworkButtons.Disconnect].interactable = false;
            _networkButtons[(int)Globals.Enums.NetworkButtons.Host].interactable = false;
            _networkButtons[(int)Globals.Enums.NetworkButtons.Join].interactable = false;
            _networkButtons[(int)Globals.Enums.NetworkButtons.StartGame].interactable = false;

            PhotonNetwork.sendRate = 30;
            PhotonNetwork.sendRateOnSerialize = 30;
            PhotonNetwork.automaticallySyncScene = true;
        }

        private void Update()
        {
            if (PhotonNetwork.connected != _isConnected)
            {
                _isConnected = PhotonNetwork.connected;
                if (!_isConnected)
                {
                    ChatController.SetUpChat(false);
                    
                    // only change these when disconnected
                    _networkButtons[(int)Globals.Enums.NetworkButtons.Host].interactable = false;
                    _networkButtons[(int)Globals.Enums.NetworkButtons.Join].interactable = false;
                }

                _networkInputs[(int)Globals.Enums.NetworkInputs.PlayerName].interactable = _isConnected;
                _networkInputs[(int)Globals.Enums.NetworkInputs.RoomName].interactable = _isConnected;
                _networkInputs[(int)Globals.Enums.NetworkInputs.CloudRegion].interactable = !_isConnected;
                _networkInputs[(int)Globals.Enums.NetworkInputs.ChatRegion].interactable = !_isConnected;

                _networkButtons[(int)Globals.Enums.NetworkButtons.Connect].interactable = !_isConnected;
                _networkButtons[(int)Globals.Enums.NetworkButtons.Disconnect].interactable = _isConnected;
                _networkButtons[(int)Globals.Enums.NetworkButtons.StartGame].interactable = false;

            }

            _networkTexts[(int)Globals.Enums.NetworkTexts.NetworkState].text = PhotonNetwork.connectionState.ToString() + ". " + _networkState;
            _networkTexts[(int)Globals.Enums.NetworkTexts.ErrorState].text = _errorState;
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
                _errorState = "Player name must be filled";
                return false;
            }

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
            int count = PhotonNetwork.playerList.Count(
                player => player.NickName.ToLower() == _playerName.ToLower()
            );
            if (count > 1)
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

            _networkButtons[(int)Globals.Enums.NetworkButtons.Host].interactable = true;
            _networkButtons[(int)Globals.Enums.NetworkButtons.Join].interactable = true;
        }

        public override void OnJoinedRoom()
        {
            // Assert that each player has unique name.
            if (!IsPlayerNameUnique())
            {
                PhotonNetwork.LeaveRoom();
                return;
            }

            ChatController.SetUpChat(true);

            _errorState = string.Empty;
            _networkState = "Joined room";

            foreach (Selectable networkInput in _networkInputs)
            {
                networkInput.interactable = false;
            }

            _networkButtons[(int)Globals.Enums.NetworkButtons.Host].interactable = false;
            _networkButtons[(int)Globals.Enums.NetworkButtons.Join].interactable = false;

            _networkTexts[(int)Globals.Enums.NetworkTexts.RequiredToConnect].gameObject
                .SetActive(true);
            _networkTexts[(int)Globals.Enums.NetworkTexts.CurrentlyConnected].text = 
                "Currently Connected: " + PhotonNetwork.playerList.Length;

            if (PhotonNetwork.isMasterClient)
            {
                _networkButtons[(int)Globals.Enums.NetworkButtons.StartGame].interactable = true;
            }
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

            // Check if specified room exists
            bool roomExist = PhotonNetwork.GetRoomList().Count(room => room.Name == _roomName) > 0;
            if (roomExist)
            {
                _errorState = "Specified room alredy exists";
            }
            else
            {
                RoomOptions roomOpts = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
                PhotonNetwork.CreateRoom(_roomName, roomOpts, TypedLobby.Default);
            }
        }

        public void JoinRoom()
        {
            _errorState = string.Empty;
            if (!IsReadyToPair())
                return;

            // Check if specified room exists
            bool roomExist = PhotonNetwork.GetRoomList().Count(room => room.Name == _roomName) > 0;
            if (roomExist)
                PhotonNetwork.JoinRoom(_roomName);
            else
                _errorState = "Room doesn't exist";
        }

        public void StartGame()
        {
            if (PhotonNetwork.playerList.Length == 1)
            {
                PhotonNetwork.LoadLevel((int)Globals.Enums.SceneIndex.Game);
            }
            else
            {
                _errorState = "Wait for opponnent to connect";
            }
        }

        public void Return()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene((int)Globals.Enums.SceneIndex.MainMenu);
        }

        public void Exit()
        {
            PhotonNetwork.Disconnect();
            Application.Quit();
        }

        #endregion connect methods
    }
}