using System;
using UnityEngine;
using Steamworks;

namespace TAHL.WAVE_BENDER
{
    public class SteamLobbyManager: MonoBehaviour
    {
        public GameObject ConnectionLoading = null;
        public GameObject[] NetworkPanels = null;
        private bool joinedSteamLobby = false;
        private CSteamID lobbyId; 
        // Start is called before the first frame update
        void Start()
        {
            if (!SteamManager.Initialized)
            {
                throw new Exception("Steam Manager not initialized");
            }

            SteamAPI.Init();
            SteamAPICall_t steamCall = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
            
            Callback<LobbyCreated_t>.Create(LobbyCreated);
            Callback<LobbyEnter_t>.Create(LobbyEntered);
        }

        void Update()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            SteamAPI.RunCallbacks();
        }

        private void LobbyEntered(LobbyEnter_t enteredLobby)
        {
            if (enteredLobby.m_ulSteamIDLobby != 0)
            {
                lobbyId = new CSteamID(enteredLobby.m_ulSteamIDLobby);
                joinedSteamLobby = true;
            }

            Globals.GameProperties.isSteamLobbyEntered = true;
            if (Globals.GameProperties.isNetworkLoaded)
            {
                Globals.GameProperties.isSteamLobbyEntered = true;
                ConnectionLoading.SetActive(false);
                NetworkPanels[(int)Globals.Enums.NetworkPanels.Connection].SetActive(false);
                NetworkPanels[(int)Globals.Enums.NetworkPanels.Room].SetActive(true);
            }
        
            SteamMatchmaking.GetLobbyData(lobbyId, "");
            Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdate);
        }

        private void LobbyDataUpdate(LobbyDataUpdate_t lobbyData)
        {
            Debug.Log(lobbyData);
        }

        private void LobbyCreated(LobbyCreated_t created)
        {
        }

    }
}