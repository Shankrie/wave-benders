using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class PlayerPrefLoader : MonoBehaviour
    {

        #region Public Variables

        public Globals.PlayerPrefFlags playerPrefFlag;

        #endregion public

        #region Private Variables

        private InputField _inputField;
        private Dropdown _dropdownField;

        #endregion private

        private void Start()
        {
            switch ((int)playerPrefFlag)
            {
                case (int)Globals.PlayerPrefFlags.PlayerName:
                    string name = SetPrefFromText(SetPlayerName);
                    SetupPlayerName(name);
                    break;
                case (int)Globals.PlayerPrefFlags.GameRoomName:
                    SetPrefFromText(SetGameRoomName);
                    break;
                case (int)Globals.PlayerPrefFlags.CloudRegion:
                    SetPrefFromDropdown();
                    break;
                case (int)Globals.PlayerPrefFlags.ChatRegion:
                    SetPrefFromDropdown();
                    break;
                default:
                    break;
            }
            
        }

        #region Bind Events and add default values

        /// <summary>
        /// Set input field default value from player preferences and return result
        /// </summary>
        /// <returns></returns>
        private string SetPrefFromText(UnityAction<string> onChange)
        {
            // Add onvalueChange event to input field
            _inputField = GetComponent<InputField>();
            _inputField.onValueChanged.AddListener(onChange);

            // if there's no saved player preference for current flag
            if (!PlayerPrefs.HasKey(Globals.PlayerPrefs[(int)playerPrefFlag]))
            {
                return string.Empty;
            }
            string defaultName = PlayerPrefs.GetString(Globals.PlayerPrefs[(int)playerPrefFlag]);
            _inputField.text = defaultName;
            return defaultName;
        }

        private void SetPrefFromDropdown()
        {
            _dropdownField = GetComponent<Dropdown>();
            _dropdownField.onValueChanged.AddListener(SetRegion);

            // if there's no saved player preference then add default
            if (!PlayerPrefs.HasKey(Globals.PlayerPrefs[(int)playerPrefFlag]))
            {
                return;
            }

            int defaultIndex = PlayerPrefs.GetInt(Globals.PlayerPrefs[(int)playerPrefFlag]);
            _dropdownField.value = defaultIndex;
        }

        private void SetupPlayerName(string name)
        {
            PhotonNetwork.playerName = name;
            PhotonNetwork.player.NickName = name;
        }

        #endregion

        #region On Change Events

        public void SetPlayerName(string value)
        {
            PhotonNetwork.playerName = _inputField.text;
            PhotonNetwork.player.NickName = _inputField.text;
            PlayerPrefs.SetString(Globals.PlayerPrefs[(int)playerPrefFlag], _inputField.text);
        }

        public void SetGameRoomName(string value)
        {
            PlayerPrefs.SetString(Globals.PlayerPrefs[(int)playerPrefFlag], _inputField.text);
        }

        public void SetRegion(int index)
        {
            PlayerPrefs.SetInt(Globals.PlayerPrefs[(int)playerPrefFlag], _dropdownField.value);
        }

        #endregion
    }
}