using UnityEngine;

namespace TAHL.WAVE_BENDER
{

    public class BackgroundMusicPlayer : MonoBehaviour
    {

        // public GameObject gameObject;
        // Use this for initialization
        void Start()
        {

            PlayBackgroundTheme();
            // if (gameObject.name == "Seal") PlaySealBark();
            // if (gameObject.name == "Penguin") PlayPenguinBattleCry();
        }

        // Update is called once per frame
        void Update()
        {

        }
        void PlayBackgroundTheme()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = Resources.Load("Ocean") as AudioClip;
            audioSource.Play();
            audioSource.loop = true;
        }
    }
}