using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupName : NetworkBehaviour {
    public TextMesh tm;


	// Use this for initialization
	void Start () {
		if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        Debug.Log(GlobalVariables.playerCount);
        Debug.Log(GlobalVariables.playerName);
        if (GlobalVariables.playerName == "")
        {
            if (GlobalVariables.playerCount % 2 == 1)
                tm.text = "Player B";
            else
                tm.text = "Player A";

        }
        else
        {
            if(tm.text == "")
                tm.text = GlobalVariables.playerName;
        }        
        GlobalVariables.playerCount++;
	}
	
}
