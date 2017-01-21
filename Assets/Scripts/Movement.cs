using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        float translation = CrossPlatformInputManager.GetAxis("Horizontal") * 5 * Time.deltaTime;
        transform.Translate(translation, 0, 0);
    }


}
