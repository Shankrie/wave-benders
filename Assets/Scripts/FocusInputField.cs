using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusInputField : InputField {

    public bool ForceFocus = false;

    public override void OnDeselect(BaseEventData eventData)
    {
        if (ForceFocus)
        {
            ForceFocus = false;
        }
        else
        {
            base.OnDeselect(eventData);
        }
    }
}
