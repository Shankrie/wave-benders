using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key {

    public Transform KeyObject;
    public int KeyIndex;

    public Key(Transform keyObject, int keyIndex)
    {
        KeyObject = keyObject;
        KeyIndex = keyIndex;
    }


}
