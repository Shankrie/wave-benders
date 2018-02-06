using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public Texture BackgText;

    // Make cursor visible on the screen
    void Update()
    {
        Cursor.visible = true;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width * .375f, Screen.height * .5f, Screen.width * .25f, Screen.height * .05f), "Start Game"))
        {
            SceneManager.LoadScene((int)Globals.SceneIndex.Game);
        }
        else if (GUI.Button(new Rect(Screen.width * .375f, Screen.height * .575f, Screen.width * .25f, Screen.height * .05f), "Credits"))
        {
            SceneManager.LoadScene((int)Globals.SceneIndex.Credits);
        }
        else if (GUI.Button(new Rect(Screen.width * .375f, Screen.height * .65f, Screen.width * .25f, Screen.height * .05f), "Exit"))
        {
            Application.Quit();
        }
    }
}