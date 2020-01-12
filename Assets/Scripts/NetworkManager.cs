using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

namespace TAHL.WAVE_BENDER
{

  public class NetworkManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
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

    public List<Region> Regions = null;
    public Action RegionChanged = null;
    public string CurrentRegion = String.Empty;

    #region Private variables

    private LoadBalancingClient _loadBalancingClient = null;
    private List<Region> _regions = null;
    private TextMeshProUGUI _networkStateLog = null;

    private string _networkState = string.Empty;
    private string _errorState = string.Empty;
    private string _playerName = string.Empty;
    private string _roomName = string.Empty;

    private string _connectionState = string.Empty;

    private const string LocalIp = "127.0.0.1";
    private const string APP_ID = "24d3a1f4-57b0-46d1-8e62-a96a1aa64df8";
    private bool _isConnected = false;

    #endregion vars

    #region Private methods

    private void Start() {
        _networkStateLog = NetworkState.GetComponentInChildren<TextMeshProUGUI>();
        this._loadBalancingClient = new LoadBalancingClient();
        this._loadBalancingClient.AppId = APP_ID;
        this._loadBalancingClient.AddCallbackTarget(this);
        this._loadBalancingClient.ConnectToNameServer();
    }

    private void Update()
    {
        if (this._loadBalancingClient != null)
        {
            this._loadBalancingClient.Service();

            string state = _loadBalancingClient.State.ToString();
            if (this._networkState != state)
            {
                this._networkState = state;
                this.StateText.text = state;
            }
        }
    }

    #endregion

    #region Connect, Create or Join Rooms, Disconnect on click methods

    // public void ConnectToServer()
    // {
    //     ReConnectBtn.interactable = false;
    //     _errorState = string.Empty;

    //     PhotonNetwork.LogLevel = PunLogLevel.Full;

    //     // PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
    //     // PhotonNetwork.PhotonServerSettings.AppSettings.Server = "127.0.0.1";
    //     // PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
    //     PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = APP_ID;
    //     // this._loadBalancingClient.
    //     // this._loadBalancingClient.ConnectToNameServer();

    //     PhotonNetwork.ConnectToBestCloudServer();

    // }

    // void OnConnectionFail(DisconnectCause cause)
    // {
    //     this.ConnectTitle.text = "Failed to Connect To server";
    //     ReConnectBtn.interactable = true; 
    // }

    // void OnFailedToConnectToPhoton(DisconnectCause cause)
    // {
    //     this.ConnectTitle.text = "Failed to Connect To server";
    //     ReConnectBtn.interactable = true; 
    // }

    // public override void OnConnected()
    // {
    //     this.ConnectTitle.text = "Join or Create Room";
    // }

    // public override void OnConnectedToMaster()
    // {
    //     this._loadBalancingClient.ConnectToNameServer();
    //     // this._loadBalancingClient.LoadBalancingPeer.OpGetRegions(APP_ID);
    //     // {
    //     //     if(this._loadBalancingClient.RegionHandler != null)
    //     //     {
    //     //         Debug.Log(this._loadBalancingClient.RegionHandler.EnabledRegions.Count);
    //     //     }
    //     // }
    //     this.ConnectTitle.text = "Join or Create Room";
    //     NetworkPanels[(int)NetworkPanelEnum.Conenction].SetActive(false);
    //     NetworkPanels[(int)NetworkPanelEnum.Room].SetActive(true);
    //     // PhotonNetwork.CreateRoom("room");

    //     // PhotonNetwork.JoinLobby();
    //     // if(PhotonNetwork.NetworkingClient.RegionHandler != null)
    //     // {
    //     //     List<Region> regions = PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions;
    //     //     foreach(Region region in regions)
    //     //     {
    //     //         Debug.Log(region.ToString());
    //     //     }
    //     // }
    // }

    // public override void OnJoinRandomFailed(short returnCode, string message)
    // {
    //     if(returnCode == 32760) {
    //         PhotonNetwork.CreateRoom("MyRoom");
    //     }
    //     Debug.Log(returnCode);
    //     Debug.Log(message);
    // }

    // public override void OnJoinRoomFailed(short returnCode, string message)
    // {
    //     Debug.Log(returnCode);
        
    //     Debug.Log(message);
    // }

    // public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    // {
    //     Debug.Log(cause);
    // }

    // public void StartGame()
    // {
    //   if (PhotonNetwork.PlayerList.Length == 2)
    //   {
    //     PhotonNetwork.LoadLevel((int)Globals.Enums.SceneIndex.Game);
    //   }
    //   else
    //   {
    //     _errorState = "Wait for opponnent to connect";
    //   }
    // }

    // public void Return()
    // {
    //   PhotonNetwork.Disconnect();
    //   SceneManager.LoadScene((int)Globals.Enums.SceneIndex.MainMenu);
    // }

    public void Exit()
    {
      PhotonNetwork.Disconnect();
      Application.Quit();
    }

    public void LogNetworkState(string log)
    {
        NetworkState.SetActive(true);
        _networkStateLog.text = log;
    }

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        Globals.GameProperties.isNetworkLoaded = true;
        if (Globals.GameProperties.isSteamLobbyEntered)
        {
            ConnectionLoading.SetActive(false);
            NetworkPanels[(int)Globals.Enums.NetworkPanels.Connection].SetActive(false);
            NetworkPanels[(int)Globals.Enums.NetworkPanels.Room].SetActive(true);
        }

        CurrentRegion = _loadBalancingClient.CloudRegion;
        if (RegionChanged != null)
        {
            RegionChanged();
        }
    }


    public void ConnectToRegion(string region)
    {
        if (!_loadBalancingClient.ConnectToRegionMaster(region.ToLower()))
        {
            Debug.Log(String.Format("Region could not be changed to {0}", region));
            RegionChanged();
            return;
        }

        ConnectionLoading.SetActive(true);
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected(" + cause + ")");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived");
        regionHandler.PingMinimumOfRegions(this.OnRegionPingCompleted, null);
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnJoinedLobby()
    {
    }

    public void OnLeftLobby()
    {
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        this._loadBalancingClient.OpCreateRoom(new EnterRoomParams());
    }

    public void OnLeftRoom()
    {
    }


    /// <summary>A callback of the RegionHandler, provided in OnRegionListReceived.</summary>
    /// <param name="regionHandler">The regionHandler wraps up best region and other region relevant info.</param>
    private void OnRegionPingCompleted(RegionHandler regionHandler)
    {
        Regions = regionHandler.EnabledRegions;
        
        Debug.Log("OnRegionPingCompleted " + regionHandler.BestRegion);
        Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
        this._loadBalancingClient.ConnectToRegionMaster(regionHandler.BestRegion.Code);
    }

    #endregion connect methods
  }
}