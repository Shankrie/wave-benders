using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    /// <summary>
    /// Basic concept of ShowSuggestions is to show dropdown of suggestions when input is filled up
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    class ShowSuggestions: MonoBehaviour
    {
        #region variables

        public FocusInputField RoomNameInput = null;

        private List<string> _filteredOptions = new List<string>();

        private Dropdown _allRooms = null;
        private string _text = string.Empty;

        private bool _hidden = true;
        private bool _ignoreSelected = true;
        private bool _first = true;

        #endregion

        #region Private methods

        private void Start()
        {
            if(RoomNameInput == null)
            {
                enabled = false;
                throw new Exception("Room Name Input Field is required component");
            }

            _allRooms = GetComponent<Dropdown>();
            _allRooms.onValueChanged.AddListener(OnSelect);
        }

        private void UpdateSuggestions()
        {
            if(_text.Length > 2)
            {
                // clear left options
                _allRooms.ClearOptions();
                _filteredOptions.Clear();

                // filter room list by current roomInfo name
                foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList()) {
                    if(Regex.Match(roomInfo.Name, _text, RegexOptions.IgnoreCase).Success)
                    {
                        _filteredOptions.Add(roomInfo.Name);
                    }
                }

                // if no found then 
                if (_filteredOptions.Count == 0)
                {
                    if(!_hidden)
                        ShowDropdown(false);
                    return;
                }

                // Set interactable and add filtered options
                _ignoreSelected = true;
                _allRooms.interactable = true;
                _allRooms.AddOptions(_filteredOptions);

                // Remove Checkbox by setting and removing dropdown selected item
                _allRooms.options.Add(new Dropdown.OptionData() { text = "" });
                _allRooms.value = _allRooms.options.Count - 1;
                _allRooms.options.RemoveAt(_allRooms.options.Count - 1);

                if(_hidden)
                {
                    ShowDropdown(true);
                }
            }
            else if(!_hidden)
            {
                ShowDropdown(false);
            }
        }

        private void ShowDropdown(bool show)
        {
            // set dropdown to non interactable 
            _allRooms.interactable = show;

            // don't ya loose focus of input
            RoomNameInput.ForceFocus = true;

            if (show)
                _allRooms.Show();
            else
                _allRooms.Hide();

            _hidden = !show;
        }

        #endregion

        #region Value Change And Select Event methods

        public void OnTextValueChange(string text)
        {
            // Ignore first time changes from other scripts like PlayerPrefLoader
            if (_first)
            {
                _first = false;
                return;
            }
            _text = text;
            UpdateSuggestions();

            // Again reselect input field
            RoomNameInput.Select();
        }

        public void OnSelect(int index)
        {
            if(_ignoreSelected)
            {
                _ignoreSelected = false;
                return;
            }

            string text = _filteredOptions[index];
            _text = text;

            RoomNameInput.text = text;
            RoomNameInput.Select();
        }

        #endregion
    }
}
