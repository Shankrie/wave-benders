using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;

public class AddSteamFriendsInSidebar : MonoBehaviour
{
    public GameObject FriendPrefab;
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
        AddFriends();
    }

    void AddFriends() {
        for(int i = 0; i < SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll); i++)
        {
            CSteamID id = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
            string friendName = SteamFriends.GetFriendPersonaName(id);
            GameObject GO = Instantiate(FriendPrefab);
            TextMeshProUGUI friendNameUI = GO.GetComponentInChildren<TextMeshProUGUI>();
            friendNameUI.text = "Friend: " + friendName; 
            GO.transform.parent = transform;
            RectTransform rectTransform = GO.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            // rectTransform.anchoredPosition = new Vector2(0, 0);
            // rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, i * 150, 0);
            // rectTransform.
        }

    }

}
