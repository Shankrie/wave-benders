using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small script fixing rect transform height and width
/// </summary>
public class FixedRect : MonoBehaviour {

    public Vector2 fixedPosition = Vector2.zero;
    public Vector2 fixedSize = Vector2.zero;

	// Use this for initialization
	void Start () {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null)
            throw new Exception("Rect Transform not found in gameobject");
        rect.anchoredPosition = fixedPosition;
        rect.sizeDelta = fixedSize;
	}

}
