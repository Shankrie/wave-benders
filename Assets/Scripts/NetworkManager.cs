using Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class NetworkManager : PunBehaviour
    {
        #region Public buttons

        public Transform MasterClientPosition;
        public Transform ClientPosition;

        public Button HostButton;
        public Button JoinButton;

        public GameObject waitingForPlayer;

        #endregion buttons

        #region Private variables

        private GameObject _networkingGUI;

        private string Version = "1.0";
        private string LocalIp = "127.0.0.1";
        private string _connectState = "";

        private int Port = 5055;

        private bool _joinHostBtnsActive = false;

        #endregion vars

        #region Unity default methods

        private void Start()
        {
            HostButton.interactable = false;
            JoinButton.interactable = false;

            waitingForPlayer.SetActive(false);

            _networkingGUI = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkGUI);
            PhotonNetwork.sendRate = 30;
            PhotonNetwork.sendRateOnSerialize = 30;

            // Sync scene means when player connects he get's all latest objects positions and stuff like dat.
            ConnectToServer();
        }

        private void Update()
        {
            if (PhotonNetwork.connected == true && _joinHostBtnsActive == false)
            {
                _joinHostBtnsActive = true;
                HostButton.interactable = true;
                JoinButton.interactable = true;
            }
            else if (PhotonNetwork.connected == false && _joinHostBtnsActive == true)
            {
                _joinHostBtnsActive = false;
                HostButton.interactable = false;
                JoinButton.interactable = false;
            }
        }

        private void OnGUI()
        {
            if (_networkingGUI.activeSelf)
                GUI.TextField(new Rect(0, 0, 250, 50), _connectState);
        }

        #endregion

        #region Connect, Create or Join Rooms, Disconnect, Reconnect onclick methods

        public void ConnectToServer()
        {
            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.autoJoinLobby = true;

            // Connect to cloud server
            //PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, "v1.0");

            // Connect to local server
            PhotonNetwork.ConnectToMaster(LocalIp, Port, string.Empty, Version);
            _connectState = "Connecting to: " + LocalIp + ":" + Port;
        }

        public void OnConnectedToServer()
        {
            _connectState = "Connected";
        }

        public void HostRoom()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                _connectState = "Connect first!";
                return;
            }

            // Check if room name is entered
            string roomName = PlayerPrefs.GetString(Globals.PUNKeys.gameRoomName);
            if (string.IsNullOrEmpty(roomName))
            {
                _connectState = "Room name must be filled";
                return;
            }

            RoomOptions roomOpts = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(roomName, roomOpts, TypedLobby.Default);
        }

        public void JoinRoom()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                _connectState = "Connect first!";
                return;
            }

            // Check if room name and player name are entered
            string roomName = PlayerPrefs.GetString(Globals.PUNKeys.gameRoomName);
            if (string.IsNullOrEmpty(roomName))
            {
                _connectState = "Room name must be filled";
                return;
            }

            // Check if specified room exists and if player with same name doesn't exist
            bool roomExist = PhotonNetwork.GetRoomList().Count(room => room.Name == roomName) > 0;
            if (roomExist)
                PhotonNetwork.JoinRoom(roomName);
            else
                _connectState = "Room doesn't exist";
        }

        public void OnReconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public void OnReturn()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene((int)Globals.SceneIndex.MainMenu);
        }

        #endregion connect methods

        #region Photon inherited methods

        public override void OnJoinedLobby()
        {
            _connectState = "Joined in lobby";
        }

        public override void OnJoinedRoom()
        {
            // Spawn player from Resources folder
            if (PhotonNetwork.isMasterClient)
            {
                waitingForPlayer.SetActive(true);
                PhotonNetwork.Instantiate(Globals.PrefabNames.MasterClient, MasterClientPosition.position, Quaternion.identity, 0);
            }
            else
            {
                PhotonNetwork.Instantiate(Globals.PrefabNames.Client, ClientPosition.position, Quaternion.identity, 0);
            }
            _networkingGUI.SetActive(false);
        }

        public override void OnDisconnectedFromPhoton()
        {
            ConnectToServer();
        }

        #endregion
    }
}