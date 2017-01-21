using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupName : NetworkBehaviour {

	void Start()
    {
        if (isLocalPlayer)
        {
            
            GetComponent<Movement>().enabled = true;
            setLocalObjName(GlobalVariables.clientName);
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
