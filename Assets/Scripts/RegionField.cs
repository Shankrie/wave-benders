using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    [RequireComponent(typeof(Dropdown))]
    public class RegionField: MonoBehaviour
    {
        public Globals.Enums.RegionType regionType = 0;
        
        private Dropdown _regionDropdown;

        private int _selectedIndex = 0;

        // private Type[] regionTypes = new Type[] { typeof(CloudRegionCode), typeof(Globals.Enums.ChatRegionCode) };

        private void Awake()
        {
            _regionDropdown = GetComponent<Dropdown>();

            // string[] regionNames = Enum.GetNames(regionTypes[(int)regionType]);
            // _regionDropdown.AddOptions(regionNames.ToList());
        }

        public int GetSelectedIndex()
        {
            return _selectedIndex;
        }

        public void OnChange(int index)
        {
            _selectedIndex = index;
        }

    }
}