using System;
using System.Collections;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{

    public static class SteamFriendAnimBools
    {
        public static string Active = "Active";
    }

    public class SteamFriendsManager : MonoBehaviour
    {
        public GameObject FriendsContainer;
        public GameObject FriendPrefab;
        public Texture2D FriendNotFound;
        private bool _friendsAdded = false;
        private  Color ONLINE_COLOR = new Color(0f, 1f, .05f, 1f);
        private  Color OFFLINE_COLOR = new Color(.47f, .47f, .47f, 1f);

        private bool joinedSteamLobby = false;
        private CSteamID lobbyId; 
        // Start is called before the first frame update
        void Start()
        {
            if (!SteamManager.Initialized)
            {
                throw new Exception("Steam Manager not initialized");
            }

            if (!FriendPrefab)
            {
                throw new Exception("Steam Friend Prefab not set");
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
        
            AddFriends();

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

        private void InviteFriend(CSteamID userId)
        {
            if (joinedSteamLobby) {
                SteamMatchmaking.InviteUserToLobby(lobbyId, userId);
            }
        }

        public void AddFriends() {
            if (_friendsAdded)
            {
                return;
            }
            
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
            Animator animator;
            for(int i = 0; i < friendCount; i++)
            {
                // Get friend CSTEAM id, personal name
                CSteamID id = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                string friendName = SteamFriends.GetFriendPersonaName(id);

                // Get friend personal profile picture
                Texture2D avatarTexture;
                if (Globals.Variables.Cache.ContainsKey(id))
                {
                    avatarTexture = Globals.Variables.Cache[id];
                }
                else
                {
                    avatarTexture = Globals.Methods.GetFriendTexture(id);
                    if (avatarTexture == null)
                    {
                        avatarTexture = FriendNotFound;
                    }
                    else
                    {
                        Globals.Variables.Cache.Add(id, avatarTexture);
                    }
                }

                // assign for default picture
                if (avatarTexture.Equals(null)) {
                    avatarTexture = this.FriendNotFound; 
                }

                // Check if online
                EPersonaState state = SteamFriends.GetFriendPersonaState(id);
                GameObject GO = Instantiate(FriendPrefab);
                animator = GO.GetComponent<Animator>();
                GO.transform.SetParent(FriendsContainer.transform);

                // Couldn't do it with animation somewhy ? so hardcoding in the state check 
                Image profileImg = GO.transform.GetChild(0).GetComponent<Image>();
                Image onlineStatusImg = GO.transform.GetChild(1).GetComponent<Image>();
                TextMeshProUGUI name = GO.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                Transform actions = GO.transform.GetChild(3);

                // set friend online status and if online move as first element 
                if (state == EPersonaState.k_EPersonaStateOnline)
                {
                    onlineStatusImg.color = ONLINE_COLOR;
                    actions.gameObject.SetActive(true);
                    GO.transform.SetAsFirstSibling();
                    
                    Button[] buttons = actions.GetComponentsInChildren<Button>();

                    // invite button
                    buttons[0].onClick.AddListener(delegate() {
                        InviteFriend(id);
                    });

                    buttons[1].onClick.AddListener(delegate() {
                        // SteamFriends.ReplyToFriendMessage();
                    });
                    
                } 
                else
                {
                    name.color = OFFLINE_COLOR;
                    actions.gameObject.SetActive(false);
                    onlineStatusImg.color = OFFLINE_COLOR;   
                }

                // when image found then create sprite and to it
                if (profileImg)
                {
                    Sprite sprite = Sprite.Create(avatarTexture, new Rect(0, 0, 64, 64), Vector2.zero, 100, 0, SpriteMeshType.Tight, new Vector4(64, 64, 64, 64), false);
                    profileImg.sprite = sprite;
                }

                TextMeshProUGUI friendNameUI = GO.GetComponentInChildren<TextMeshProUGUI>();
                friendNameUI.text = friendName; 
                GO.name = friendName;

                // Reset prefab rect transform size 
                RectTransform rectTransform = GO.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1, 1, 1);
            }
            _friendsAdded = true;
        }

        private void InviteFriendToGame(CSteamID friendId)
        {
            SteamFriends.InviteUserToGame(friendId, "Yikes"); 
        }
    }


}