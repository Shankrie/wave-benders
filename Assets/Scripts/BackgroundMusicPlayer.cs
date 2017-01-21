using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{

   // public GameObject gameObject;
	// Use this for initialization
	void Start () {

        PlayBackgroundTheme();
      // if (gameObject.name == "Seal") PlaySealBark();
      // if (gameObject.name == "Penguin") PlayPenguinBattleCry();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void PlayBackgroundTheme()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("Ocean") as AudioClip;
        audioSource.Play();
    }
    void PlaySealBark()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("sealBarking") as AudioClip;
        audioSource.Play();
    }
    void PlayPenguinBattleCry()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("penguinBattleCry") as AudioClip;
        audioSource.Play();
    }
}
