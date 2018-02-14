using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class Key
    {
        public Transform KeyObject;
        public int KeyIndex;

        public Key(Transform keyObject, int keyIndex)
        {
            KeyObject = keyObject;
            KeyIndex = keyIndex;
        }
    }
}