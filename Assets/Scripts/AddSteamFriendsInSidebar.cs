using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class AddSteamFriendsInSidebar : MonoBehaviour
    {
        public GameObject FriendPrefab;
        public Texture2D FriendNotFound;
        private bool _friendsAdded = false;
        // Start is called before the first frame update
        void Start()
        {
            if(!SteamManager.Initialized)
            {
                throw new Exception("Steam Manager not initialized");
            }
            if(!FriendPrefab)
            {
                throw new Exception("Steam Friend Prefab not set");
            }
        }

        public void AddFriends() {
            if(_friendsAdded)
            {
                return;
            }
            
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
            for(int i = 0; i < friendCount; i++)
            {
                CSteamID id = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                string friendName = SteamFriends.GetFriendPersonaName(id);
                Texture2D avatarTexture;
                if (Globals.Variables.Cache.ContainsKey(id)) {
                    avatarTexture = Globals.Variables.Cache[id];
                } else {
                    avatarTexture = Globals.Methods.GetFriendTexture(id);
                    if (avatarTexture == null) {
                        avatarTexture = FriendNotFound;
                    } else {
                        Globals.Variables.Cache.Add(id, avatarTexture);
                    }
                }

                if (avatarTexture.Equals(null)) {
                    avatarTexture = this.FriendNotFound; 
                }

                GameObject GO = Instantiate(FriendPrefab);
                RawImage img = GO.GetComponentInChildren<RawImage>();
                img.texture = avatarTexture;

                TextMeshProUGUI friendNameUI = GO.GetComponentInChildren<TextMeshProUGUI>();
                friendNameUI.text = friendName; 
                GO.transform.parent = transform;
                GO.name = friendName;
                RectTransform rectTransform = GO.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1, 1, 1);
            }
            _friendsAdded = true;
        }
    }


}