﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupName : NetworkBehaviour {

    public GameObject Wave;

   // public WaveMover waveMover;

	// Use this for initialization
	void Start () {
        NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        
        if (isLocalPlayer)
        {
            GlobalVariables.playerCount++;
            
            GetComponent<Movement>().enabled = true;
            setLocalObjName(GlobalVariables.clientName);

            if (GlobalVariables.playerCount == 2)
            {
                //Wave.GetComponent<WaveMover>().enabled = true;
                Instantiate(Wave);
                Debug.Log("Hey");
            }
        }

        

    }

    public void setLocalObjName(string newName)
    {
        if(GlobalVariables.isHostOn == true && GlobalVariables.hostInitialized == false)
        {
            GetComponentInChildren<TextMesh>().text = GlobalVariables.hostName;
            GlobalVariables.hostInitialized = true;
        }
        else if(GlobalVariables.isClientOn == true && GlobalVariables.clientInitialized == false)
        {
            GetComponentInChildren<TextMesh>().text = GlobalVariables.clientName;
            GlobalVariables.clientInitialized = true;
        }
        
    }
	
}
