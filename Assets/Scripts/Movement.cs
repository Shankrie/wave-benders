using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour {

    public GameObject KeyGenObject;
    private KeyGenerator keyGen;
    private int index = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (GameObject.Find("KeyGen(Clone)") == null)
            return;

        keyGen = GameObject.Find("KeyGen(Clone)").GetComponent<KeyGenerator>();

        //Debug.Log("hostmove " + (keyGen.hostMove && !NetworkServer.active));
        //Debug.Log("nothostmove " + (!keyGen.hostMove && NetworkServer.active));

        if (keyGen.hostMove && !NetworkServer.active)
            return;

        if (!keyGen.hostMove && NetworkServer.active)
            return;

        if (index < keyGen.spawnedKeys.Count)
        {
            Key key = keyGen.spawnedKeys[index];
            KeyCode keyToEnter = keyGen.keyCodesDic[key.KeyIndex];

            if (Input.GetKeyDown(keyToEnter))
            {
                keyGen.CmdGreyOutKey(index);

                index++;
            }

            if (index == keyGen.spawnedKeys.Count)
            {
                keyGen.CmdDeflectWave();
                keyGen.CmdDestroySpawnedKeys();
                keyGen.generateKeys();
                index = 0;
            }
                
        }
    }

    [Command]
    public void CmdSetDeflectWave()
    {
        RpcSetDeflectWave();
    }

    [ClientRpc]
    public void RpcSetDeflectWave()
    {
        GlobalVariables.sendWaveAway = true;
    }
}
