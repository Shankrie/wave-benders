using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupName : NetworkBehaviour {

    public GameObject Wave;
    public GameObject KeyGen;
    bool waveInstantiated = false;
   // public WaveMover waveMover;

    // Use this for initialization
    void Start () {

        NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        GlobalVariables.playerCount++;
        if (GlobalVariables.playerCount == 2 && waveInstantiated == false)
        {
            //Wave.GetComponent<WaveMover>().enabled = true;
            Instantiate(Wave);
            waveInstantiated = true;
            
            if (!NetworkServer.active)
                CmdSpawn();
        }
        if (isLocalPlayer)
        {
            
            
            GetComponent<Movement>().enabled = true;
            setLocalObjName(GlobalVariables.clientName);


        }

        Debug.Log(GlobalVariables.playerCount);
        Debug.Log("Hey");



    }

    [Command]
    void CmdSpawn()
    {
        GameObject go = (GameObject)Instantiate(KeyGen);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    public void setLocalObjName(string newName)
    {
        if(GlobalVariables.isHostOn == true && GlobalVariables.hostInitialized == false)
        {
            //GetComponentInChildren<TextMesh>().text = GlobalVariables.hostName;
            GlobalVariables.hostInitialized = true;
        }
        else if(GlobalVariables.isClientOn == true && GlobalVariables.clientInitialized == false)
        {
            //GetComponentInChildren<TextMesh>().text = GlobalVariables.clientName;
            GlobalVariables.clientInitialized = true;
        }
        
    }
    
}
