using System;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class SteamName : MonoBehaviour
    {
        public Texture2D FriendNotFound;
        public ChatController ChatController;
        // Start is called before the first frame update
        void Start()
        {
            if (!SteamManager.Initialized)
            {
                throw new Exception("Steam Manager not initialized");
            }
            if (!FriendNotFound)
            {
                throw new Exception("Friend not found texture not initialized");
            }

            CSteamID id = SteamUser.GetSteamID();
            string name = SteamFriends.GetPersonaName();
            TextMeshProUGUI playerName = GetComponentInChildren<TextMeshProUGUI>();
            playerName.text = name;

            PlayerPrefs.SetString(Globals.PUNKeys.playerName, name);
            PlayerPrefs.SetString(Globals.PUNKeys.userId, id.m_SteamID.ToString());
            ChatController.InitializeChatConnection();

            Texture2D avatarTexture = null;
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

            RawImage playerAvatarPic = GetComponentInChildren<RawImage>();
            playerAvatarPic.texture = avatarTexture;
            
        }

    }
}