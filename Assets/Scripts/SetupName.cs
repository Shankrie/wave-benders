using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupName : NetworkBehaviour {
    public TextMesh tm;
    public GameObject Wave;
   // public WaveMover waveMover;

	// Use this for initialization
	void Start () {
        Debug.Log(GlobalVariables.isHostOn);
        Debug.Log(GlobalVariables.isClientOn);
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

	    if (GlobalVariables.isHostOn && GlobalVariables.isClientOn)
	    {
            //Wave.GetComponent<WaveMover>().enabled = true;
	        Instantiate(Wave);
            Debug.Log("Hey");
	    }

	    GlobalVariables.playerCount++;
	}
	
}
