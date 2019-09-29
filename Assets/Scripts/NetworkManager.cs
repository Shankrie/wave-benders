using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace TAHL.WAVE_BENDER
{

  enum NetworkPanelEnum
  {
    Conenction = 0,
    Room = 1
  }

  public class NetworkManager : MonoBehaviourPunCallbacks
  {
    #region Public variables
    public ChatController ChatController = null;
    public GameObject[] NetworkPanels = null;
    public TextMeshProUGUI ConnectTitle = null;
    public TextMeshProUGUI StateText  = null;
    public Button ReConnectBtn = null;
    public GameObject ConnectionLoading = null;
    public GameObject NetworkState = null;

    #endregion

    #region Private variables

    private LoadBalancingClient loadBalancingClient = null;
    private TextMeshProUGUI NetworkStateLog = null;

    private string _networkState = string.Empty;
    private string _errorState = string.Empty;
    private string _playerName = string.Empty;
    private string _roomName = string.Empty;

    private string _connectionState = string.Empty;

    private const string LocalIp = "127.0.0.1";

    private bool _isConnected = false;

    #endregion vars

    #region Private methods

    private void Start()
    {
        NetworkStateLog = NetworkState.GetComponentInChildren<TextMeshProUGUI>();
        this.loadBalancingClient = new LoadBalancingClient();
        this.loadBalancingClient.AppId = "24d3a1f4-57b0-46d1-8e62-a96a1aa64df8";
        this.loadBalancingClient.AddCallbackTarget(this);
        this.loadBalancingClient.ConnectToNameServer();

        ConnectToServer();
    }

    private void Update()
    {
        if (this.loadBalancingClient != null)
        {
            this.loadBalancingClient.Service();

            string state = loadBalancingClient.State.ToString();
            if (this._networkState != state)
            {
                this._networkState = state;
                this.StateText.text = state;
            }
        }
    }

    private bool IsReadyToPair()
    {
      if (!PhotonNetwork.IsConnectedAndReady)
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

    #endregion

    #region Connect, Create or Join Rooms, Disconnect on click methods

    public void ConnectToServer()
    {
        ReConnectBtn.interactable = false;
        _errorState = string.Empty;

        PhotonNetwork.LogLevel = PunLogLevel.Full;

        // PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
        // PhotonNetwork.PhotonServerSettings.AppSettings.Server = "127.0.0.1";
        // PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
        // PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;
        PhotonNetwork.ConnectUsingSettings();
    }

    void OnConnectionFail(DisconnectCause cause)
    {
        this.ConnectTitle.text = "Failed to Connect To server";
        ReConnectBtn.interactable = true; 
    }

    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        this.ConnectTitle.text = "Failed to Connect To server";
        ReConnectBtn.interactable = true; 
    }

    public override void OnConnected()
    {
        this.ConnectTitle.text = "Join or Create Room";
    }

    public override void OnConnectedToMaster()
    {
        this.ConnectTitle.text = "Join or Create Room";
        NetworkPanels[(int)NetworkPanelEnum.Conenction].SetActive(false);
        NetworkPanels[(int)NetworkPanelEnum.Room].SetActive(true);
        // PhotonNetwork.CreateRoom("room");

        PhotonNetwork.JoinLobby();
    }


    public void JoinRandomRoom()
    {
        Debug.Log("join random room");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {

      Debug.Log("Joined Room");
      // Assert that each player has unique name.
      // if (!IsPlayerNameUnique())
      // {
      //     PhotonNetwork.LeaveRoom();
      //     return;
      // }

      // // ChatController.SetUpChat(true);

      // _errorState = string.Empty;
      // _networkState = "Joined room";

      // foreach (Selectable networkInput in _networkInputs)
      // {
      //     networkInput.interactable = false;
      // }

      // _networkButtons[(int)Globals.Enums.NetworkButtons.Host].interactable = false;
      // _networkButtons[(int)Globals.Enums.NetworkButtons.Join].interactable = false;

      // _networkTexts[(int)Globals.Enums.NetworkTexts.RequiredToConnect].gameObject
      //     .SetActive(true);

      // if (PhotonNetwork.IsMasterClient)
      // {
      //     _networkButtons[(int)Globals.Enums.NetworkButtons.StartGame].interactable = true;
      // 
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(returnCode == 32760) {
            PhotonNetwork.CreateRoom("MyRoom");
        }
        Debug.Log(returnCode);
        Debug.Log(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode);
        
        Debug.Log(message);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.Log(cause);
    }

    public void StartGame()
    {
      if (PhotonNetwork.PlayerList.Length == 2)
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

    public void LogNetworkState(string log)
    {
        NetworkState.SetActive(true);
        NetworkStateLog.text = log;
    }

    #endregion connect methods
  }
}