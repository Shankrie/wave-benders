using System.Text.RegularExpressions;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class KeyController : MonoBehaviour
    {

        /// <summary>
        ///     Is player on right or left direction(1, -1);
        /// </summary>
        /// <Default>
        ///     Player is on right side
        /// </Default>
        public int PlayerDirection = 1;
        /// <summary>
        /// Key input progress
        /// </summary>
        public int Progress = 0;

        public int SetKeyCount { set { _keyCount = value; } }
        /// <summary>
        /// Checks if it's player turn to input keys before wave crash on him or her.
        /// </summary>
        public bool MyTurn = false;

        /// <summary>
        /// Return true if player controls pohoton view
        /// </summary>
        public bool ControlsView { get { return _pw.isMine; } }

        public KeyController[] SetKeyControllers { set { _playersKeyController = value; } }


        private KeyController[] _playersKeyController = null;

        private KeyGenerator _keyGen;
        private CountDown _countDown;
        private WaveMover _waveMover;
        private GameController _gameController;
        private DeathController _deathController;
        private PhotonView _pw;

        private int _keyCount = 0;

        private bool _waveFlooded = false;

        void Awake()
        {
            GameObject keyGenObject = GameObject.FindGameObjectWithTag(Globals.Tags.KeyGen);
            GameObject waveObject = GameObject.FindGameObjectWithTag(Globals.Tags.Wave);
            GameObject gameControllObject = GameObject.FindGameObjectWithTag(Globals.Tags.GameController);

            _keyGen = keyGenObject.GetComponent<KeyGenerator>();
            _countDown = keyGenObject.GetComponent<CountDown>();
            _waveMover = waveObject.GetComponent<WaveMover>();
            _gameController = gameControllObject.GetComponent<GameController>();

            _deathController = GetComponent<DeathController>();
            _pw = GetComponent<PhotonView>();

            // setting keygen and countdown movement script when player controls view
            if (_pw.isMine)
            {
                _countDown.SetKeyController = this;
                if (!PhotonNetwork.isMasterClient)
                {
                    StartCountDownCall();
                }
            }
            else
            {
                // Disable box collider when to avoid unnecessary collision
                BoxCollider2D collider = GetComponent<BoxCollider2D>();
                collider.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_pw.isMine || _waveFlooded || Progress >= _keyCount)
                return;

            if (Input.anyKeyDown && Regex.Match(Input.inputString, @"^[a-zA-Z0-9]$").Success)
            {
                if (!MyTurn)
                {
                    RaiseWaveKeyPressed();
                }
                else
                {
                    DeflectWaveKeyPressed();
                }
            }
        }

        private void OnGUI()
        {
            string keys = "";
            if (_pw.isMine)
            {
                GUI.TextArea(new Rect(Screen.width - (Screen.width * 0.3f), 0, 100, 50), "Master: " + MyTurn.ToString());
                GUI.TextArea(new Rect(Screen.width - (Screen.width * 0.1f), 0, 100, 50), "View Mine: " + keys);
            }
            else
            {
                GUI.TextArea(new Rect(Screen.width * 0, 0, 100, 50), "Master: " + MyTurn.ToString());
 
                GUI.TextArea(new Rect(Screen.width * 0.2f, 0, 100, 50), "View Mine: " + keys);
            }
        }


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
            if (_keyGen.IsKeySpawnedKey(Input.inputString.ToLower(), Progress))
            {
                _keyGen.GrayOutKey(MyTurn, Progress);
                Progress++;

                // Flip the wave if progessed current level
                if (Progress == _keyCount)
                {
                    _pw.RPC("DeflectWaveRPC", PhotonTargets.All, _waveMover.transform.position);
                }
            }
            // Move the wave faster to player and don't let player to do anything
            else if (Regex.Match(Input.inputString.ToLower(), "[a-z0-9]").Success)
            {
                _pw.RPC("ForceFloodWaveRPC", PhotonTargets.All);
            }
        }

        private void RaiseWaveKeyPressed()
        {
            // When it's not yuor turn you can increase wave speeed 
            if (_keyGen.IsKeySpawnedKey(Input.inputString.ToLower(), Progress))
            {
                _keyGen.GrayOutKey(MyTurn, Progress);
                Progress++;

                if (Progress == _keyCount)
                {
                    _pw.RPC("IncreaseWaveSpeedRPC", PhotonTargets.All);

                }
            }
        }



        /// <summary>
        /// Starts countdown localy
        /// </summary>
        public void StartCountDown()
        {
            _countDown.StartCountDown();
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
                _pw.RPC("InitializingRPC", PhotonTargets.All);
        }

        public void ResetLevelCall()
        {
            _pw.RPC("ResetLevelRPC", PhotonTargets.All);
        }

        [PunRPC]
        public void StartCountDownRPC()
        {
            _countDown.StartCountDown();
        }

        [PunRPC]
        public void InitializingRPC()
        {
            _gameController.GetPlayerData();

            // Reset level and destroy leftovers if exist
            _keyGen.Level = 0;
            _keyGen.DestroySpawnedKeys();


            // get new keys for client
            int[] keys = _keyGen.GetRandomKeys();


            _playersKeyController = gameObject.GetComponentsByTag<KeyController>(Globals.Tags.Player);
            foreach(GameObject go in GameObject.FindGameObjectsWithTag(Globals.Tags.Player))
            {
                KeyController controller = go.GetComponent<KeyController>();
                controller.MyTurn = controller.ControlsView == PhotonNetwork.isMasterClient;
                controller.PlayerDirection = controller.MyTurn ? 1 : -1;
                controller.Progress = 0;
                controller.SetKeyCount = Globals.Difficulty.DifficultyLevels[_keyGen.Level];

                if (controller.ControlsView)
                {
                    _keyGen.PaintKeys(keys, controller.MyTurn);
                }
                if (!GameObject.ReferenceEquals(go, gameObject))
                {
                    go.GetComponent<KeyController>().SetKeyControllers = _playersKeyController;
                }
            }

            _waveMover.enabled = true;
            _waveMover.gameObject.SetActive(true);
        }

        [PunRPC]
        public void DeflectWaveRPC(Vector3 wavePosition)
        {
            _keyGen.IncreaseLevel();
            _keyGen.DestroySpawnedKeys();

            int[] keys = _keyGen.GetRandomKeys();

            foreach (KeyController controller in _playersKeyController)
            {
                controller.MyTurn = !controller.MyTurn;
                controller.Progress = 0;
                controller.SetKeyCount = Globals.Difficulty.DifficultyLevels[_keyGen.Level];
                if (controller.ControlsView)
                {
                    _keyGen.PaintKeys(keys, controller.MyTurn);
                }
            }

            // call wave component to turn to other side
            _waveMover.transform.position = wavePosition;
            _waveMover.deflectWave();
        }

        [PunRPC]
        public void IncreaseWaveSpeedRPC()
        {
            _waveMover.IncreaseSpeed();
        }

        [PunRPC]
        public void ForceFloodWaveRPC()
        {
            _waveFlooded = true;
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

            if (MyTurn)
            {
                _deathController.playerHaveLost(PlayerDirection);
            }
            else
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag(Globals.Tags.Player);
                foreach (GameObject player in players)
                {
                    if (!GameObject.ReferenceEquals(player, gameObject))
                    {
                        player.GetComponent<DeathController>().playerHaveLost(PlayerDirection);
                        break;
                    }
                }
            }
        }

        [PunRPC]
        public void ResetLevelRPC()
        {
            Progress = 0;

            // Reset wave position scale and facing direction
            _waveMover.ResetWave();
            _waveMover.enabled = false;
            _waveFlooded = false;

            _keyGen.DestroySpawnedKeys();

            _gameController.ResetLevel();
            StartCountDown();
        }
    }
}