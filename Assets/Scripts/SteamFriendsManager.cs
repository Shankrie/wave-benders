using System;
using System.Collections;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
        private Color OFFLINE_COLOR = new Color(.47f, .47f, .47f, 1f);
        private List<CSteamID> _friendsId = new List<CSteamID>();
        private List<string> _friendsName = new List<string>();
        private List<Texture2D> _friendsAvatar = new List<Texture2D>();

        private bool joinedSteamLobby = false;
        private CSteamID lobbyId; 
        // Start is called before the first frame update
        void Start()
        {
            if (!FriendPrefab)
            {
                throw new Exception("Steam Friend Prefab not set");
            }
        }

        private void InviteFriend(CSteamID userId)
        {
            if (joinedSteamLobby) {
                SteamMatchmaking.InviteUserToLobby(lobbyId, userId);
            }
        }

        public void SetFriendsData()
        {
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
            for(int i = 0; i < friendCount; i++)
            {
                // Get friend CSTEAM id, personal name
                CSteamID id = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                string name = SteamFriends.GetFriendPersonaName(id);

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

                if (avatarTexture.Equals(null)) {
                    avatarTexture = this.FriendNotFound; 
                }

                _friendsId.Add(id);
                _friendsName.Add(name);
                _friendsAvatar.Add(avatarTexture);
            }
        }

        public void AddFriends() 
        {
            if (_friendsAdded)
            {
                UpdateFriendsList();
                return;
            }
            
            Animator animator;
            for(int i = 0; i < _friendsId.Count; i++)
            {
                // Get friend CSTEAM id, personal name
                CSteamID id = _friendsId[i]; 
                string friendPersonalName = _friendsName[i]; 
                Texture2D avatarTexture = _friendsAvatar[i];

                // Get friend personal profile picture
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
                friendNameUI.text = friendPersonalName; 
                GO.name = friendPersonalName;

                // Reset prefab rect transform size 
                RectTransform rectTransform = GO.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1, 1, 1);
            }
            _friendsAdded = true;
        }
        
        public void UpdateFriendsList()
        {
            for(int i = 0; i < _friendsId.Count; i++)
            {
                // Get friend CSTEAM id, personal name
                CSteamID id = _friendsId[i]; 
                string friendPersonalName = _friendsName[i]; 
                Texture2D avatarTexture = _friendsAvatar[i];

                // Get friend personal profile picture
                // Check if online
                EPersonaState state = SteamFriends.GetFriendPersonaState(id);

                Transform FriendTransform = FriendsContainer.transform.GetChildByName(friendPersonalName);
                if (FriendTransform == null)
                {
                    continue;
                }

                // Couldn't do it with animation somewhy ? so hardcoding in the state check 
                Image onlineStatusImg = FriendTransform.GetChild(1).GetComponent<Image>();

                // set friend online status and if online move as first element 
                if (state == EPersonaState.k_EPersonaStateOnline)
                {
                    onlineStatusImg.color = ONLINE_COLOR;
                } 
                else
                {
                    onlineStatusImg.color = OFFLINE_COLOR;   
                }
            }

        }

        private void InviteFriendToGame(CSteamID friendId)
        {
            SteamFriends.InviteUserToGame(friendId, "Yikes"); 
        }
    }
}