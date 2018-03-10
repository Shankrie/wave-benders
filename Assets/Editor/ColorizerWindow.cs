using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ColorizerWindow : EditorWindow
{
    Color color = Color.white;

    [MenuItem("Window/Colorizer")]
    public static void ShowWindow()
    {
        GetWindow<ColorizerWindow>("Colorizer");
    }

    void OnGUI()
    {
        GUILayout.Label("Change color of GUIs backgrounds ", EditorStyles.boldLabel);
        color = EditorGUILayout.ColorField("Color", color);

        if(GUILayout.Button("Colorize GUIs"))
        {
            foreach(GameObject background in GameObject.FindGameObjectsWithTag("GUIBackground"))
            {
                background.GetComponent<Image>().color = color;
            }
        }
    }
}