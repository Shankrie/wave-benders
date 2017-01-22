using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KeyGenerator : NetworkBehaviour {

    public GameObject Keyboard;
    public Transform KeysPosition;

    public Dictionary<int, KeyCode> keyCodesDic = new Dictionary<int, KeyCode>();

    public List<Key> spawnedKeys = new List<Key>();
    private List<Transform> allKeys = new List<Transform>();

    [SyncVar]
    public int difficulty = 0;
    private Dictionary<int, int> difficultyDic = new Dictionary<int, int>();

    [SyncVar]
    public bool hostMove = true;

    [SyncVar]
    public bool deflectWave = false;

    void Start () {

        difficultyDic.Add(0, 3);
        difficultyDic.Add(1, 3);
        difficultyDic.Add(2, 4);
        difficultyDic.Add(3, 4);
        difficultyDic.Add(4, 5);
        difficultyDic.Add(5, 5);
        difficultyDic.Add(6, 6);
        difficultyDic.Add(7, 6);
        difficultyDic.Add(8, 7);
        difficultyDic.Add(9, 7);

        keyCodesDic.Add(0, KeyCode.Q);
        keyCodesDic.Add(1, KeyCode.W);
        keyCodesDic.Add(2, KeyCode.E);
        keyCodesDic.Add(3, KeyCode.R);
        keyCodesDic.Add(4, KeyCode.T);
        keyCodesDic.Add(5, KeyCode.Y);
        keyCodesDic.Add(6, KeyCode.U);
        keyCodesDic.Add(7, KeyCode.I);
        keyCodesDic.Add(8, KeyCode.O);
        keyCodesDic.Add(9, KeyCode.P);
        keyCodesDic.Add(10, KeyCode.A);
        keyCodesDic.Add(11, KeyCode.S);
        keyCodesDic.Add(12, KeyCode.D);
        keyCodesDic.Add(13, KeyCode.F);
        keyCodesDic.Add(14, KeyCode.G);
        keyCodesDic.Add(15, KeyCode.H);
        keyCodesDic.Add(16, KeyCode.J);
        keyCodesDic.Add(17, KeyCode.K);
        keyCodesDic.Add(18, KeyCode.L);
        keyCodesDic.Add(19, KeyCode.Z);
        keyCodesDic.Add(20, KeyCode.X);
        keyCodesDic.Add(21, KeyCode.C);
        keyCodesDic.Add(22, KeyCode.V);
        keyCodesDic.Add(23, KeyCode.B);
        keyCodesDic.Add(24, KeyCode.N);
        keyCodesDic.Add(25, KeyCode.M);

        foreach (Transform key in Keyboard.transform)
        {
            allKeys.Add(key);
        }

        if (!NetworkServer.active)
            generateKeys();        
    }

    public void generateKeys()
    {
        CmdPaint();
    }

    [ClientRpc]
    void RpcPaint(int[] selectedKeys)
    {
        float keySpacing = 0.7f;

        float positionOffset = 0;

        int numberOfKeys = difficultyDic[difficulty];
        if (numberOfKeys % 2 == 0)
            positionOffset = keySpacing / 2;

        for (int i = 0; i < selectedKeys.Length; i++)
        {
            Vector3 startPosition = new Vector3(KeysPosition.position.x - (numberOfKeys / 2 * keySpacing) + positionOffset, KeysPosition.position.y, KeysPosition.position.z);

            Vector3 position = new Vector3(startPosition.x + i * keySpacing, startPosition.y, startPosition.z);

            Transform randomKey = allKeys[selectedKeys[i]];
            Transform keyObject = Instantiate(randomKey, position, randomKey.rotation);
            spawnedKeys.Add(new Key(keyObject, selectedKeys[i]));
        }
    }

    [Command]
    void CmdPaint()
    {
        System.Random rnd = new System.Random();

        int numberOfKeys = difficultyDic[difficulty];

        int[] selKeys = new int[numberOfKeys];
        for (int i = 0; i < numberOfKeys; i++)
        {
            int randomKeyIndex = rnd.Next(1, allKeys.Count);
            selKeys[i] = randomKeyIndex;
        }

        NetworkIdentity objNetId = GetComponent<NetworkIdentity>();
        if (!NetworkServer.active)
            objNetId.AssignClientAuthority(connectionToClient);    // assign authority to the player who is changing the color
        
        RpcPaint(selKeys);                                    // usse a Client RPC function to "paint" the object on all clients

        if (!NetworkServer.active)
            objNetId.RemoveClientAuthority(connectionToClient);    // remove the authority from the player who changed the color
    }

    [Command]
    public void CmdDestroySpawnedKeys()
    {
        RpcDestroySpawnedKeys();
    }

    [ClientRpc]
    private void RpcDestroySpawnedKeys()
    {
        foreach (Key key in spawnedKeys)
        {
            Destroy(key.KeyObject.gameObject);
        }

        spawnedKeys = new List<Key>();
        hostMove = !hostMove;
    }

    [Command]
    public void CmdGreyOutKey(int index)
    {
        RpcGreyOutKey(index);
    }

    [ClientRpc]
    private void RpcGreyOutKey(int index)
    {
        spawnedKeys[index].KeyObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
    }

    [Command]
    public void CmdDeflectWave()
    {
        RpcDeflectWave();
    }

    [ClientRpc]
    private void RpcDeflectWave()
    {
        deflectWave = true;
    }
}
