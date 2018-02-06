using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{

    // Use this for initialization
    public Texture BackgText;
    string Programming_credits = "\n\tProgramming\n\nHenrikas Jasiūnas\nAurimas Mikėnas\nLukas Tutkus";
    string Sound_credits = "\n\tSounds\n\nAurimas Mikėnas\nThanks to free sound libraries:\nhttps://freesound.org/";
    string Animation_credits = "\n\tAnimation\n\nHenrikas Jasiūnas\nLukas Tutkus";
    //string 
    GUIStyle Credits_style = new GUIStyle();
    void Update()
    {
        Cursor.visible = true;
    }

    void OnGUI()
    {

        Credits_style.fontSize = 18;
        Credits_style.normal.textColor = Color.white;


        GUI.Label(new Rect(Screen.width * .375f, Screen.height * .15f, Screen.width * .25f, Screen.height * .25f), Programming_credits, Credits_style);
        GUI.Label(new Rect(Screen.width * .375f, Screen.height * .35f, Screen.width * .25f, Screen.height * .25f), Sound_credits, Credits_style);
        GUI.Label(new Rect(Screen.width * .375f, Screen.height * .60f, Screen.width * .25f, Screen.height * .25f), Animation_credits, Credits_style);

        if (GUI.Button(new Rect(Screen.width * .375f, Screen.height * 0.9f, Screen.width * .25f, Screen.height * .05f), "Back"))
        {
            SceneManager.LoadScene((int)Globals.SceneIndex.MainMenu);
        }


    }
}
