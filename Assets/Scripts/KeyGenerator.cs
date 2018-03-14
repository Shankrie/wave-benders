using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class KeyGenerator : MonoBehaviour
    {
        public List<Key> spawnedKeys = new List<Key>();
        public Transform KeysPosition;
        public int Level { get { return _level; } set { _level = value; } }

        private Sprite[] _sprites;

        private int _level = 0;
        private bool _initialized = false;

        void Start()
        {
            _sprites = Resources.LoadAll<Sprite>("keys");
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
                Vector3 startPosition = new Vector3(
                    KeysPosition.position.x - (numberOfKeys / 2 * keySpacing) + positionOffset, 
                    KeysPosition.position.y, 
                    KeysPosition.position.z
                );

                Vector3 position = new Vector3(startPosition.x + i * keySpacing, startPosition.y, startPosition.z);

                // Create key gameobject
                Transform randomKeyObject = new GameObject().transform;
                randomKeyObject.position = position;
                randomKeyObject.rotation = Quaternion.identity;
                randomKeyObject.gameObject.name = _sprites[keys[i]].name;

                // add sprite renderer component to gameobject
                SpriteRenderer renderer = randomKeyObject.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = _sprites[keys[i]];
                renderer.color = myTurn ?
                    Globals.ColorsByTurn[(int)Globals.ColorTurnIndex.myTurn] :
                    Globals.ColorsByTurn[(int)Globals.ColorTurnIndex.oponnentTurn];
                spawnedKeys.Add(new Key(randomKeyObject, keys[i]));
            }
        }

        public int[] GetRandomKeys()
        {
            System.Random rnd = new System.Random();

            int numberOfKeys = Globals.Difficulty.DifficultyLevels[_level];

            int[] keys = new int[numberOfKeys];
            for (int i = 0; i < numberOfKeys; i++)
            {
                int randomKeyIndex = rnd.Next(0, _sprites.Length - 1);
                keys[i] = randomKeyIndex;
            }

            return keys;
        }

        public void GrayOutKey(bool myTurn, int key)
        {
            if (spawnedKeys.Count == 0)
                return;

            // get and remove key from spawnedKeys list
            GameObject keyRef = spawnedKeys[key].KeyObject.gameObject;

            // gray out that key
            SpriteRenderer renderer = keyRef.GetComponent<SpriteRenderer>();
            renderer.color = myTurn ?
                Globals.InactiveColorsByTurn[(int)Globals.ColorTurnIndex.myTurn] :
                Globals.InactiveColorsByTurn[(int)Globals.ColorTurnIndex.oponnentTurn];
        }

        public void IncreaseLevel()
        {
            if (_level < Globals.Difficulty.MAX_DIFF_LEVEL)
                _level++;
        }

        public void DestroySpawnedKeys()
        {
            while (spawnedKeys.Count != 0)
            {
                Key firstKey = spawnedKeys[0];
                spawnedKeys.RemoveAt(0);
                Destroy(firstKey.KeyObject.gameObject);
            }
        }
        
        public bool IsKeySpawnedKey(string key, int progress)
        {
            if(spawnedKeys.Count <= progress)
            {
                throw new System.Exception("Error. Spawned key is out of bounds");
            }

            // is first gameobject name char is equal to key char
            return spawnedKeys[progress].KeyObject.name[0] == key[0];
        }
    }
}