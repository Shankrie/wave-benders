using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class Movement : MonoBehaviour, IPunObservable {
    /// <summary>
    ///     Is player on right or left direction(1, -1);
    /// </summary>
    /// <Default>
    ///     Player is on right side
    /// </Default>
    public int PlayerDirection = 1;
    /// <summary>
    /// Checks if it's player turn to input keys before wave crash on him or her.
    /// </summary>
    public bool IsMyTurn { get { return _myTurn; } }

    /// <summary>
    /// Return true if player controls pohoton view
    /// </summary>
    public bool ControlsView { get { return _pw.isMine; } }


    private KeyGenerator _keyGen;
    private CountDown _countDown;
    private WaveMover _waveMover;
    private GameController _gameController;
    private DeathController _deathController;
    private PhotonView _pw;

    private int[] _selectedKeys = null;
    private int _index = 0;
    private int _level = 0;
    private int _progress = 0;
    private bool _waveFlooded = false;
    private bool _lockedTurn = false;

    private bool _myTurn = false;

    void Start()
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
        if(_pw.isMine)
        {
            _keyGen.SetViewMovement = this;
            _countDown.SetViewMovement = this;
            if(PhotonNetwork.isMasterClient)
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

        _myTurn = !PhotonNetwork.isMasterClient;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!_pw.isMine || _waveFlooded)
            return;

        if(Input.anyKeyDown && _selectedKeys != null && _selectedKeys.Length > 0 && _progress < _selectedKeys.Length)
        {
            int currentKey = _selectedKeys[_progress];
            if (!_myTurn)
            {
                // When it's not yuor turn you can increase wave speeed 
                if (Input.GetKeyDown(Globals.keyCodes[currentKey]))
                {
                    _progress++;
                    _keyGen.GrayOutFirstKey(_myTurn);

                    if(_progress == _selectedKeys.Length)
                    {
                        _pw.RPC("IncreaseWaveSpeedRPC", PhotonTargets.All);

                    }
                }
            }
            else
            {
                // When it's your turn you must finish 
                if (Input.GetKeyDown(Globals.keyCodes[currentKey]))
                {
                    _progress++;
                    _keyGen.GrayOutFirstKey(_myTurn);

                    // Flip the wave if progessed current level
                    if (_progress == _selectedKeys.Length)
                    {
                        _pw.RPC("DeflectWaveRPC", PhotonTargets.All, _waveMover.transform.position);
                    }
                }
                // Move the wave faster to player and don't let player to do anything
                else if (Regex.Match(Input.inputString, "[a-z0-9]").Success)
                {
                    _pw.RPC("ForceFloodWaveRPC", PhotonTargets.All);
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag(Globals.Tags.Wave))
        {
            _pw.RPC("WaveFloodRPC", PhotonTargets.All);
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
        // only send keys from master client
        if (!PhotonNetwork.isMasterClient)
            return;

        _waveMover.enabled = true;
        _waveMover.gameObject.SetActive(true);

        _pw.RPC("InitializingRPC", PhotonTargets.All);
    } 

    public void DeflectWaveCall()
    {
        _pw.RPC("DeflectWaveRPC", PhotonTargets.All, _waveMover.transform.position);
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
        _progress = 0;
        _keyGen.SetLevel = 0;

        // reverse turn
        _myTurn = !_myTurn;

        // paint new keys
        _keyGen.SetLevel = 0;
        _keyGen.spawnedKeys.Clear();
        _selectedKeys = _keyGen.GetRandomKeys();
        _keyGen.PaintKeys(_selectedKeys, _myTurn);
    }

    [PunRPC]
    public void DeflectWaveRPC(Vector3 wavePosition)
    {
        _keyGen.IncreaseLevel();
        _progress = 0;

        // reverse turn
        _myTurn = !_myTurn;

        // call wave component to turn to other side
        _waveMover.transform.position = wavePosition;
        _waveMover.deflectWave();

        // paint new keys
        _keyGen.spawnedKeys.Clear();
        _selectedKeys = _keyGen.GetRandomKeys();
        _keyGen.PaintKeys(_selectedKeys, _myTurn);
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
    }

    /// <summary>
    /// RPC called when the Wave hits the player
    /// </summary>
    [PunRPC]
    public void WaveFloodRPC()
    {
        _waveFlooded = true;
        if (_myTurn)
        {
            _deathController.playerHaveLost(PlayerDirection);
        }
    }

    [PunRPC]
    public void ResetLevelRPC()
    {
        _myTurn = !PhotonNetwork.isMasterClient;
        _progress = 0;
        _waveFlooded = false;
        _selectedKeys = null;

        _gameController.ResetLevel();
        StartCountDown();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {

        }
        else
        {
 
        }
    }
}
