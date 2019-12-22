using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TAHL.WAVE_BENDER
{

    public static partial class ServerAnimatorParams
    {
        public static string DisabledTrigger = "Disabled";
        public static string NormalTrigger = "Normal";
    }

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Animator))]
    public class ChangePhotonServer : MonoBehaviour
    {
        public GameObject ServersContent = null;
        public GameObject ServerNamePrefab = null;
        public GameObject Dropdown = null;
        private Button _button;
        private Animator _animator;
        private NetworkManager _networkManager;
        private string _currentRegionName = String.Empty;
        private string _lastRegionName = String.Empty;
        private bool _regionsInitialized = false;
        private bool  _isDropdownOpen = false;

        private Dictionary<string, GameObject> _regionsGamobjects = new Dictionary<string, GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            GameObject networkManagerGO = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkManager); 
            _button = GetComponent<Button>();
            _animator = GetComponent<Animator>();
            // _animator.

            if(networkManagerGO == null)
            {
                throw new Exception("Cannot find Network Manager in ChangePhotonServer");
            }
            if (Dropdown == null)
            {
                throw new Exception("Dropdown object was not assigned in ChangePhotonServer");

            }

            // get network manager script and assign action OnRegionsResponse a delegate
            _networkManager = networkManagerGO.GetComponent<NetworkManager>();
            _networkManager.RegionChanged = delegate(string currentRegion)
            {
                TextMeshProUGUI currentServerName = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                currentServerName.text = currentRegion.ToUpper().Split('/')[0];
                _currentRegionName = currentRegion.ToUpper().Split('/')[0];

                SetAsSelected(currentRegion, 0.05f);

                // _animator.SetTrigger(ServerAnimatorParams.NormalTrigger);
                _button.interactable = true;
                InitializeRegions(_networkManager.Regions, _networkManager.CurrentRegion);
            };

        }


        private void InitializeRegions(List<Region> regions, string currentRegion) {
            if (regions == null || regions.Count == 0 || _regionsInitialized)
            {
                return;
            }

            TextMeshProUGUI currentServerName = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            currentServerName.text = currentRegion.ToUpper().Split('/')[0];
            _currentRegionName = currentRegion.ToUpper().Split('/')[0];

            _regionsInitialized = true;

            // Create Region prefab and add to ScrollView Content as childs
            foreach(Region region in regions)
            {
                GameObject regionNameGO = Instantiate(ServerNamePrefab, Vector4.zero, Quaternion.identity, ServersContent.transform);
                Button btn = regionNameGO.GetComponent<Button>();

                string regionName = region.Code.ToUpper();
                _regionsGamobjects.Add(regionName, regionNameGO);
                if (regionName == _currentRegionName)
                {
                    SetAsSelected(regionName, 0.05f);
                }

                TextMeshProUGUI regionNameText = regionNameGO.GetComponentInChildren<TextMeshProUGUI>();
                regionNameText.text = regionName;
                btn.onClick.AddListener(delegate {
                    OnSelectServer(regionNameText.text);
                });
            }

        }

        public void ToggleDropdown()
        {
            if (this._regionsInitialized)
            {
                GameObject currentRegionGO;
                if (!_isDropdownOpen && _regionsGamobjects.TryGetValue(_currentRegionName, out currentRegionGO))
                {
                }

                _isDropdownOpen = !_isDropdownOpen;
                Dropdown.SetActive(_isDropdownOpen);
            }
        }

        private void SetAsSelected(string regionName, float imageColorAlpha)
        {
            GameObject GO;
            if (!_regionsGamobjects.TryGetValue(regionName, out GO))
            {
                return;
            }

            // Button btn = GO.GetComponent<Button>();
            Image img = GO.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, imageColorAlpha);
        }

        public void OnSelectServer(string regionName)
        {
            SetAsSelected(regionName, 0.22f);
            _currentRegionName = regionName; 

            _animator.SetTrigger(ServerAnimatorParams.DisabledTrigger);
            _networkManager.ConnectToRegion(regionName);

            ToggleDropdown();
        }
    }
}