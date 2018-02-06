using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Text.RegularExpressions;

[RequireComponent(typeof(CountDown))]
public class KeyGenerator: MonoBehaviour {

    public GameObject Keyboard;
    public Transform KeysPosition;
    public Movement SetViewMovement { set { _mineViewMovement = value; } }
    public List<Key> spawnedKeys = new List<Key>();
    public List<Transform> allKeys = new List<Transform>();
    public int SetLevel { set { _level = value; } }

    public bool hostMove = true;
    public bool deflectWave = false;

    private CountDown _countDown = null;
    private Movement _mineViewMovement = null;

    private int _level = 0;

    private bool _initialized = false;

    void Start() {
        foreach (Transform key in Keyboard.transform)
        {
            allKeys.Add(key);
        }
    }

    public void PaintKeys(int[] keys, bool myTurn)
    {
        float keySpacing = 0.7f;

        float positionOffset = 0;

        int numberOfKeys = Globals.Difficulty.DifficultyLevels[_level];
        if (numberOfKeys % 2 == 0)
        {
            positionOffset = keySpacing / 2;
        }

        for (int i = 0; i < keys.Length; i++)
        {
            Vector3 startPosition = new Vector3(KeysPosition.position.x - (numberOfKeys / 2 * keySpacing) + positionOffset, KeysPosition.position.y, KeysPosition.position.z);

            Vector3 position = new Vector3(startPosition.x + i * keySpacing, startPosition.y, startPosition.z);

            Transform randomKey = allKeys[keys[i]];
            Transform keyObject = Instantiate(randomKey, position, randomKey.rotation);
            SpriteRenderer renderer = keyObject.GetComponent<SpriteRenderer>();
            renderer.color = !myTurn ?
                Globals.ColorsByTurn[(int)Globals.ColorTurnIndex.myTurn] :
                Globals.ColorsByTurn[(int)Globals.ColorTurnIndex.oponnentTurn];
            spawnedKeys.Add(new Key(keyObject, keys[i]));
        }
    }

    public int[] GetRandomKeys()
    {
        System.Random rnd = new System.Random();

        int numberOfKeys = Globals.Difficulty.DifficultyLevels[_level];

        int[] keys = new int[numberOfKeys];
        for (int i = 0; i < numberOfKeys; i++)
        {
            int randomKeyIndex = rnd.Next(1, allKeys.Count);
            keys[i] = randomKeyIndex;
        }

        return keys;
    }

    public void GrayOutFirstKey(bool myTurn)
    {
        if (spawnedKeys.Count == 0)
            return;

        // get and remove key from spawnedKeys list
        GameObject keyRef = spawnedKeys[0].KeyObject.gameObject;
        spawnedKeys.RemoveAt(0);

        // gray out that key
        SpriteRenderer renderer = keyRef.GetComponent<SpriteRenderer>();
        renderer.color = !myTurn ? 
            Globals.InactiveColorsByTurn[(int)Globals.ColorTurnIndex.myTurn] : 
            Globals.InactiveColorsByTurn[(int)Globals.ColorTurnIndex.oponnentTurn];
    }

    public void SetMineView(Movement movement)
    {
        _mineViewMovement = movement;
    }

    public void IncreaseLevel() {
        if(_level < Globals.Difficulty.MAX_DIFF_LEVEL)
            _level++;
    }
}
