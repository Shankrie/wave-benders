using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupName : NetworkBehaviour {
    [SyncVar]
    string playerName = "player";

	void Start()
    {
        if(isLocalPlayer)
        {
            GetComponent<Movement>().enabled = true;
        }
    }

	void OnGUI ()
    {
        playerName = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), playerName);
        if(GUI.Button(new Rect(130, Screen.height - 40, 80, 30), "Change"))
        {
            CmdChangeName(playerName);
        }
    }

    void Update()
    {
        if(isLocalPlayer)
        {
            GetComponentInChildren<TextMesh>().text = playerName;
        }
    }

    [Command]
    public void CmdChangeName(string newName)
    {
        playerName = newName;
    }
	
}
