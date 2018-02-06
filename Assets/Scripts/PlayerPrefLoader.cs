using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerPrefLoader : MonoBehaviour
{

    #region Public Variables

    public PlayerPrefFlags playerPrefFlag;
    public enum PlayerPrefFlags
    {
        PlayerName,
        GameRoomName
    }

    private bool _controllPlayerName;

    #endregion public

    #region Private Variables

    private string[] playerPrefs = new string[] { Globals.PUNKeys.playerName, Globals.PUNKeys.gameRoomName };
    InputField inputField;

    #endregion private

    #region MonoBehaviour CallBacks

    void Start()
    {
        string defaultName = "";
        inputField = GetComponent<InputField>();
        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(playerPrefs[(int)playerPrefFlag]))
            {
                defaultName = PlayerPrefs.GetString(playerPrefs[(int)playerPrefFlag]);
                inputField.text = defaultName;
            }
        }

        _controllPlayerName = playerPrefs[(int)playerPrefFlag] == Globals.PUNKeys.playerName;
        if (_controllPlayerName)
        {
            PhotonNetwork.playerName = defaultName;
            PhotonNetwork.player.NickName = defaultName;
        }
    }

    #endregion

    #region Public Methods

    public void SetPlayerName(string value)
    {
        if (_controllPlayerName)
        {
            PhotonNetwork.playerName = inputField.text;
            PhotonNetwork.player.NickName = inputField.text;
        }
        PlayerPrefs.SetString(playerPrefs[(int)playerPrefFlag], inputField.text);
    }

    #endregion
}