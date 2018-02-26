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
        public int SelectedRegionIndex = 0;

        private Dropdown _regionDropdown;

        private List<string> _regionNames;
        private int _selectedIndex = 0;


        private void Awake()
        {
            _regionDropdown = GetComponent<Dropdown>();
            _regionNames = Enum.GetNames(typeof(CloudRegionCode)).ToList();

            _regionDropdown.AddOptions(_regionNames);
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