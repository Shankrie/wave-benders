using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace TAHL.WAVE_BENDER
{
    [RequireComponent(typeof(Dropdown))]
    class SearchField: MonoBehaviour
    {
        public FocusInputField RoomNameInput = null;

        private List<string> _filteredOptions = new List<string>();

        private Dropdown _allRooms = null;
        private string _text = string.Empty;

        private bool _hidden = true;
        private bool _ignoreSelected = true;

        private bool _first = true;

        void Start()
        {
            if(RoomNameInput == null)
            {
                enabled = false;
                throw new Exception("Room Name Input Field is required component");
            }
            _allRooms = GetComponent<Dropdown>();
            _allRooms.onValueChanged.AddListener(OnSelect);
        }

        private void Update()
        {
            if(_text != RoomNameInput.text)
            {
                _text = RoomNameInput.text;

                // Ignore first time changes from other scripts like PlayerPrefLoader
                if(_first)
                {
                    _first = false;
                    return;
                }

                UpdateDropdownList();

                // Again reselect input field
                RoomNameInput.Select();
            }
        }

        private void UpdateDropdownList()
        {
            if(_text.Length > 2)
            {
                _allRooms.interactable = true;

                _allRooms.ClearOptions();
                _filteredOptions.Clear();
                foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList()) {
                    if(Regex.Match(roomInfo.Name, _text, RegexOptions.IgnoreCase).Success)
                    {
                        _filteredOptions.Add(roomInfo.Name);
                    }
                }
                _allRooms.AddOptions(_filteredOptions);

                // Add empty option, select it and remove it so there won't be any selected options
                _ignoreSelected = true;
                _allRooms.options.Add(new Dropdown.OptionData() { text = "" });
                _allRooms.value = _allRooms.options.Count - 1;
                _allRooms.options.RemoveAt(_allRooms.options.Count - 1);

                if(_hidden)
                {
                    // don't ya loose focus of input
                    RoomNameInput.ForceFocus = true;

                    _allRooms.Show();
                    _hidden = false;
                }
            }
            else if(!_hidden)
            {
                // set dropdown to non interactable 
                _allRooms.interactable = false;

                // don't ya loose focus of input
                RoomNameInput.ForceFocus = true;

                _allRooms.Hide();
                _hidden = true;
            }
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
    }
}
