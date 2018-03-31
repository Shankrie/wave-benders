using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class KeyGenerator : MonoBehaviour
    {
        public List<Key> _spawnedKeys = new List<Key>();
        public Transform KeysPosition;

        private Sprite[] _sprites;

        private string _lastWord = string.Empty;

        private const int LINE_KEYS_SPACE = 10;

        void Start()
        {
            _sprites = Resources.LoadAll<Sprite>("keys");
        }

        void LoadSprites()
        {
            _sprites = Resources.LoadAll<Sprite>("keys");

        }

        public void PaintKeys(int[] keys, bool myTurn)
        {
            float keyXSpacing = 0.7f;
            float overallSpace = keys.Sum(key => key == Globals.SpaceKeyIndex ? keyXSpacing * 5 : keyXSpacing);

            // cannot argument about this
            float additionalSpacing = overallSpace > LINE_KEYS_SPACE ? LINE_KEYS_SPACE * -0.5f : overallSpace * -0.5f;
            float occupiedSpace = 0f;
            int lineCount = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                // If character is space then in needs additional space
                if (keys[i] == Globals.SpaceKeyIndex || (i > 0 && keys[i - 1] == Globals.SpaceKeyIndex))
                {
                    additionalSpacing += keyXSpacing * 3f;
                    occupiedSpace += keyXSpacing * 3f;
                }
                else
                {
                    additionalSpacing += keyXSpacing;
                    occupiedSpace += keyXSpacing;
                }

                CreateKeyAtPosition(additionalSpacing, lineCount, keys[i], myTurn);

                if (occupiedSpace >= LINE_KEYS_SPACE)
                {
                    overallSpace -= occupiedSpace;
                    occupiedSpace = 0f;
                    additionalSpacing = overallSpace >= LINE_KEYS_SPACE ? LINE_KEYS_SPACE * -0.5f : overallSpace * -0.5f;
                    lineCount++;
                }
            }
        }

        public void CreateKeyAtPosition(float xSpace, float ySpace, int currentKey, bool myTurn)
        {
            Vector3 position = new Vector3(
                KeysPosition.position.x + xSpace,
                KeysPosition.position.y - ySpace,
                KeysPosition.position.z
            );

            // Create key gameobject
            Transform randomKeyObject = new GameObject().transform;
            randomKeyObject.position = position;
            randomKeyObject.rotation = Quaternion.identity;
            randomKeyObject.gameObject.name = Globals.keyCodes[currentKey].ToString().ToLower();

            // add sprite renderer component to gameobject
            SpriteRenderer renderer = randomKeyObject.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = _sprites[currentKey];
            renderer.color = myTurn ?
                Globals.ColorsByTurn[(int)Globals.ColorTurnIndex.myTurn] :
                Globals.ColorsByTurn[(int)Globals.ColorTurnIndex.oponnentTurn];

            _spawnedKeys.Add(new Key(randomKeyObject, currentKey));
        }

        public int[] GetSwearKeys(int level)
        {
            int numberOfKeys = Globals.Difficulty.DifficultyLevels[level];

            IEnumerable<string> words = Globals.CurseWords.Where(w => w.Length == numberOfKeys && w != _lastWord);
            int wordsCount = words.Count();
            string word = _lastWord;
            if (wordsCount != 0)
            {
                word = words.ElementAt(UnityEngine.Random.Range(0, wordsCount - 1));
                _lastWord = word;
            }

            int[] keys = new int[word.Length];
            int index = 0;
            foreach(char letter in word)
            {
                string letterToSearch = letter.ToString().ToLower();
                if (letter == ' ')
                {
                    letterToSearch = "space";
                }
                KeyValuePair<int, KeyCode> letterKeyCode = Globals.keyCodes.FirstOrDefault(
                   keyVal => keyVal.Value.ToString().ToLower() == letterToSearch
                );
                if (letterKeyCode.Equals(null))
                {
                    throw new Exception("Couldn't find keycode for letter: " + letterKeyCode);
                }
                keys[index++] = letterKeyCode.Key;
            }

            return keys;
        }

        public void GrayOutKey(bool myTurn, int key)
        {
            if (_spawnedKeys.Count == 0)
                return;

            // get and remove key from spawnedKeys list
            GameObject keyRef = _spawnedKeys[key].KeyObject.gameObject;

            // gray out that key
            SpriteRenderer renderer = keyRef.GetComponent<SpriteRenderer>();
            renderer.color = myTurn ?
                Globals.InactiveColorsByTurn[(int)Globals.ColorTurnIndex.myTurn] :
                Globals.InactiveColorsByTurn[(int)Globals.ColorTurnIndex.oponnentTurn];
        }

        public void DestroySpawnedKeys()
        {
            while (_spawnedKeys.Count != 0)
            {
                Key firstKey = _spawnedKeys[0];
                _spawnedKeys.RemoveAt(0);
                Destroy(firstKey.KeyObject.gameObject);
            }
        }
        
        public bool IsKeySpawnedKey(string keyName, int progress)
        {
            if(_spawnedKeys.Count <= progress)
            {
                throw new System.Exception("Error. Spawned key is out of bounds");
            }
            if (keyName == " ")
                keyName = "space";

            // is first gameobject name char is equal to key char
            return _spawnedKeys[progress].KeyObject.name == keyName;
        }
    }
}