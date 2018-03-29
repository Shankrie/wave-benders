using System.Text.RegularExpressions;
using UnityEngine;
using Photon;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    public class KeyController : PunBehaviour
    {
        public Text currentTurn;

        public Vector3 StartPosition;


        private KeyController[] _playersKeyControllers = null;
        private GameObject[] _players = null;

        private KeyGenerator _keyGen;
        private WaveMover _waveMover;
        private GameController _gameController;

        private Animator _animator;
        private DeathController _deathController;
        private PhotonView _pw;

        private string _loseCause = string.Empty;
        private string _animationName = string.Empty;

        private int _keyCount = 0;        
        /// <summary>
        ///     Is player on right or left direction(1, -1);
        /// </summary>
        public int _playerDirection = 1;
        public int _progress = 0;

        private bool _waveFlooded = false;
        private bool _myTurn = false;

        /// <summary>
        /// Return true if player controls pohoton view
        /// </summary>
        private bool _controlsView { get; set; } 

        void Awake()
        {
            GameObject keyGenObject = GameObject.FindGameObjectWithTag(Globals.Tags.KeyGen);
            GameObject waveObject = GameObject.FindGameObjectWithTag(Globals.Tags.Wave);
            GameObject gameControllObject = GameObject.FindGameObjectWithTag(Globals.Tags.GameController);

            _keyGen = keyGenObject.GetComponent<KeyGenerator>();
            _waveMover = waveObject.GetComponent<WaveMover>();
            _gameController = gameControllObject.GetComponent<GameController>();

            _animator = GetComponent<Animator>();
            _deathController = GetComponent<DeathController>();
            _pw = GetComponent<PhotonView>();

            currentTurn.enabled = false;
            StartPosition = transform.position;

            if (!PhotonNetwork.isMasterClient)
                _gameController.StartCountDown();

        }

        // Update is called once per frame
        void Update()
        {
            if (!_controlsView || _waveFlooded || _progress >= _keyCount)
                return;

            if (Input.anyKeyDown && Regex.Match(Input.inputString, @"^[a-zA-Z0-9 ]$").Success && _myTurn)
            {
                DeflectWaveKeyPressed();
            }
        }

        //private void OnGUI()
        //{
            //if (_controlsView)
            //{
            //    GUI.TextArea(new Rect(Screen.width - (Screen.width * 0.3f), 0, 100, 50), "My Turn: " + _myTurn.ToString());
            //    GUI.TextArea(new Rect(Screen.width - (Screen.width * 0.1f), 0, 100, 50), "My name: " + gameObject.name);
            //}
            //else
            //{
            //    GUI.TextArea(new Rect(Screen.width * 0, 0, 100, 50), "My Turn: " + _myTurn.ToString());
            //    GUI.TextArea(new Rect(Screen.width * 0.2f, 0, 100, 50), "My name: " + gameObject.name);
            //}
        //}


        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(Globals.Tags.Wave))
            {
                _pw.RPC("WaveFloodRPC", PhotonTargets.All);
            }
        }

        private void DeflectWaveKeyPressed()
        {
            // When it's your turn you must finish 
            if (_keyGen.IsKeySpawnedKey(Input.inputString.ToLower(), _progress))
            {
                _keyGen.GrayOutKey(_myTurn, _progress);
                _progress++;

                // Flip the wave if progessed current level
                if (_progress == _keyCount)
                {
                    _pw.RPC("DeflectWaveRPC", PhotonTargets.All, _keyGen.GetSwearKeys(), _waveMover.transform.position);
                }
            }
            // Move the wave faster to player and don't let player to do anything
            else if (Regex.Match(Input.inputString.ToLower(), "[a-zA-Z0-9 ]").Success)
            {
                _loseCause = "Wrong key pressed!";
                _pw.RPC("ForceFloodWaveRPC", PhotonTargets.All);
            }
        }

        private void RaiseWaveKeyPressed()
        {
            // When it's not yuor turn you can increase wave speeed 
            if (_keyGen.IsKeySpawnedKey(Input.inputString.ToLower(), _progress))
            {
                _keyGen.GrayOutKey(_myTurn, _progress);
                _progress++;

                if (_progress == _keyCount)
                {
                    _pw.RPC("IncreaseWaveSpeedRPC", PhotonTargets.All);

                }
            }
        }
      
        /// <summary>
        /// Calls RPC foreach Client to start countdownn
        /// </summary>
        public void StartCountDownCall()
        {
            _pw.RPC("StartCountDownRPC", PhotonTargets.All);
        }


        public void InitializeCall()
        {
            if (PhotonNetwork.isMasterClient)
                _pw.RPC("InitializingRPC", PhotonTargets.All, _keyGen.GetSwearKeys());
        }

        public void ResetLevelCall()
        {
            _pw.RPC("ResetLevelRPC", PhotonTargets.All);
        }

        [PunRPC]
        public void StartCountDownRPC()
        {
            _gameController.StartCountDown();
        }

        [PunRPC]
        public void InitializingRPC(int[] keys)
        {
            // Reset level and destroy leftovers if exist
            _keyGen.Level = 0;
            _keyGen.DestroySpawnedKeys();

            _playersKeyControllers = gameObject.GetComponentsByTag<KeyController>(Globals.Tags.Player);
            GameObject[] players = GameObject.FindGameObjectsWithTag(Globals.Tags.Player);
            foreach(GameObject go in players)
            {
                go.GetComponent<KeyController>().InitializePlayerController(players, keys);
            }

            currentTurn.enabled = true;
            currentTurn.text = _myTurn ? "My Turn" : "Opponent turn";

            _waveMover.enabled = true;
            _waveMover.gameObject.SetActive(true);
        }

        private void InitializePlayerController(GameObject[] players, int[] keys)
        {
            SetControllingView();

            _myTurn = _controlsView == PhotonNetwork.isMasterClient;

            _playerDirection = _myTurn ? 1 : -1;
            _progress = 0;
            _keyCount = keys.Length;
            _players = players;

            // Paint keys ant enable box colliders with players who control view
            if (_controlsView)
            {
                _keyGen.PaintKeys(keys, _myTurn);
                GetComponent<BoxCollider2D>().enabled = true;
            }

            // Set my turn
            if (_myTurn)
            {
                _playerDirection = 1;
                _animationName = "Flail";
            }
            else
            {
                _playerDirection = -1;
                _animationName = "Clap";
            }
        }

        [PunRPC]
        public void DeflectWaveRPC(int[] keys, Vector3 wavePosition)
        {
            _keyGen.DestroySpawnedKeys();

            foreach (KeyController controller in _playersKeyControllers)
            {
                controller.DeflectOnPlayerController(keys);
            }
            currentTurn.text = _myTurn ? "My Turn" : "Opponent turn";


            // call wave component to turn to other side
            _waveMover.transform.position = wavePosition;
            _waveMover.DeflectWave();
        }

        private void DeflectOnPlayerController(int[] keys)
        {
            _keyCount = Globals.Difficulty.DifficultyLevels[_keyGen.Level];
            if(_myTurn)
                _animator.SetBool(_animationName, true);

            _myTurn = !_myTurn;
            _progress = 0;
            if (_controlsView)
            {
                _keyGen.PaintKeys(keys, _myTurn);
            }
        }

        [PunRPC]
        public void IncreaseWaveSpeedRPC()
        {
            _waveMover.IncreaseSpeed();
        }

        [PunRPC]
        public void ForceFloodWaveRPC()
        {
            _waveMover.IncreaseSpeed();
            _keyGen.DestroySpawnedKeys();
        }

        /// <summary>
        /// RPC called when the Wave hits the player
        /// </summary>
        [PunRPC]
        public void WaveFloodRPC()
        {
            // Destroy all clues
            _waveFlooded = true;
            _keyGen.DestroySpawnedKeys();

            // this gameobject turn and it controls view
            if (_myTurn)
            {
                _deathController.playerHaveLost(_playerDirection);
                _gameController.SetWinner(!_pw.isMine, string.Empty);
            }
            else
            {
                _gameController.SetWinner(_pw.isMine, _loseCause);
                foreach (GameObject player in _players)
                {
                    if (!ReferenceEquals(player, gameObject))
                    {
                        player.GetComponent<DeathController>().playerHaveLost(_playerDirection);
                        break;
                    }
                }
            }
        }

        [PunRPC]
        public void ResetLevelRPC()
        {
            _progress = 0;

            // Reset wave position scale and facing direction
            _waveMover.ResetWave();
            _waveMover.enabled = false;
            _waveFlooded = false;

            _keyGen.DestroySpawnedKeys();
            _keyGen.Level = 0;
            _loseCause = string.Empty;

            if(_playersKeyControllers == null)
                _playersKeyControllers = gameObject.GetComponentsByTag<KeyController>(Globals.Tags.Player);

            foreach (KeyController controller in _playersKeyControllers)
            {
                controller.transform.position = controller.StartPosition;
                controller.currentTurn.enabled = false;
                controller.transform.rotation = Quaternion.identity;
            }

            _gameController.EnableGameEndGUI(false);
            _gameController.StartCountDown(true);
        }

        public void SetControllingView()
        {
            _controlsView = _pw.owner != null && _pw.owner.ID == PhotonNetwork.player.ID;
        }
    }
}