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

        if (keyGen.hostMove && !NetworkServer.active)
            return;

        if (!keyGen.hostMove && NetworkServer.active)
            return;

        if (index < keyGen.spawnedKeys.Count)
        {
            Key key = keyGen.spawnedKeys[index];
            KeyCode keyToEnter = keyGen.keyCodesDic[key.KeyIndex];
            //Debug.Log(keyToEnter);

            if (Input.GetKeyDown(keyToEnter))
            {
                //key.KeyObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

                keyGen.CmdGreyOutKey(index);

                index++;
            }

            if (index == keyGen.spawnedKeys.Count)
            {
                RpcSetDeflectWave();
                keyGen.CmdDestroySpawnedKeys();
            }
                
        }
    }

    [Command]
    private void CmdSetDeflectWave()
    {
        RpcSetDeflectWave();
    }

    [ClientRpc]
    private void RpcSetDeflectWave()
    {
        GlobalVariables.sendWaveAway = true;
    }
}
