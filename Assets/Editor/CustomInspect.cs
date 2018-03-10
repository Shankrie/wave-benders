using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(Canvas))]
public class CustomInspect: Editor
{
    Color color;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        color = EditorGUILayout.ColorField("Color", color);
        if(GUILayout.Button("Apply Color"))
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("GUIBackground"))
            {
                go.GetComponent<Image>().color = color;
            }
        }

    }

}
