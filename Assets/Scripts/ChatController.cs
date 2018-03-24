using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace TAHL.WAVE_BENDER
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(InputField))]
    [RequireComponent(typeof(Image))]
    public class ChatController : Photon.MonoBehaviour, IChatClientListener
    {

        #region publicVars
        public Image ScrollViewImage = null; 
        public GameObject ChatMessage = null;
        public GameObject ChatRefresh = null;

        public bool IsChatConnected { get { return _connected; } }
        #endregion publicVars

        #region privateVars

        private List<GameObject> _chatMessages = new List<GameObject>();
        private GameObject _chatContent = null;

        private NetworkManager _networkManager = null;
        private ChatClient _chatClient = null;
        private RectTransform _backgroundRect = null;
        private RectTransform _chatRect = null;
        private RectTransform _myRect = null;
        private InputField _myInputField = null;
        private Image _myImage = null;

        private string _channelName = null;
        private string _userId = null;

        private float nextServeTime;

        private const float dServeTime = 0.1f;
        private const int MAX_MSG_IN_SCREEN = 25;
        private const int OVERFLOW_SIZE = 28;

        private bool _subscribedToRoom = false;
        private bool _connected = false;

        private bool _chatExpanded = false;

        #endregion privateVars

        // Use this for initialization because after Network Manager Start method some content won't be available
        void Awake()
        {
            // Asure objects are ready
            if (ScrollViewImage == null)
                throw new Exception("Error. Chat scroll view image script not supplied");

            if (ChatMessage == null)
                throw new Exception("Error. Chat message object not supplied");

            if (ChatRefresh == null)
                throw new Exception("Error. Chat refresh button not supplied");
            ChatRefresh.SetActive(false);

            GameObject networkManagerObject = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkManager);
            if (networkManagerObject == null)
                throw new Exception("Error. No Network Manager in Scene");

            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            if (_networkManager == null)
                throw new Exception("Error. Network manager couldn't be found");

            // get chat content gameobject and input field
            _chatContent = GameObject.FindGameObjectWithTag(Globals.Tags.ChatContent);
            if (_chatContent == null)
                throw new Exception("Error. Chat content couldn't be found");

            _chatRect = _chatContent.transform.parent.parent.GetComponent<RectTransform>();
            if (_chatRect == null)
                throw new Exception("Error Chat ScrollView Rect Transform couldn't be found");

            GameObject background = transform.parent.GetChildByTag(Globals.Tags.GUIBackground);
            if (background== null)
                throw new Exception("Error. Chat background couldn't be found");
            _backgroundRect = background.GetComponent<RectTransform>();

            _myRect = GetComponent<RectTransform>();
            _myInputField = GetComponent<InputField>();
            _myImage = GetComponent<Image>();

            _myInputField.interactable = false;

            nextServeTime = Time.time;
        }

        void Update()
        {
            if (_connected)
                _chatClient.Service();
        }

        #region ChatClientInterfaceProps

        public void OnConnected()
        {
            Debug.Log("Connected");

            ChatRefresh.SetActive(false);

            _connected = true;
            _chatClient.Subscribe(new[] { _channelName });
            _chatClient.SetOnlineStatus(ChatUserStatus.Online);
        }

        public void OnDisconnected()
        {
            Debug.Log("Disonnected: " + _chatClient.DisconnectedCause.ToString());

            ChatRefresh.SetActive(true);
            _myInputField.interactable = false;
            SetUpChatColor(false);
            StopAllCoroutines();

            _connected = false;
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log("Private msg: " + message);
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log("status update: " + user + ' ' + status.ToString() + ' ' + message.ToString());
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log("Subscribed");
            _subscribedToRoom = true;

            _myInputField.interactable = true;
            _myInputField.placeholder.gameObject.SetActive(false);
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log("DebugReturn");
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("OnChatStateChange");
        }

        public void OnUnsubscribed(string[] channels)
        {
            Debug.Log("Unsubscribed");
        }

        #endregion ChatClientInterfaceProps

        #region Coroutines

        //private IEnumerator ServeMessages()
        //{
        //    while(true)
        //    {
        //        yield return new WaitForSeconds(0.1f);

        //        _chatClient.Service();
        //    }
        //}

        #endregion

        #region OnValueChange, OnClick Methods

        public void OnValueChange(string value)
        {
            if(Regex.Match(value, @"[^\r]{1}\n").Success)
            {
                Regex.Replace(value, @"\r\n", string.Empty);
                SendText(value);
                if (_chatExpanded)
                    ExpandChat(!_chatExpanded);
                return;
            }

            // Expand chat if 
            int msgHeight = (int)_myInputField.preferredHeight;
            if((!_chatExpanded && msgHeight > OVERFLOW_SIZE) || (
                _chatExpanded && msgHeight <= OVERFLOW_SIZE))
            {
                ExpandChat(!_chatExpanded);
            }
        }

        /// <summary>
        /// Expands or Decrease chat input field height and moves chat
        /// </summary>
        /// <param name="expand"></param>
        private void ExpandChat(bool expand)
        {
            _chatExpanded = expand;
            int margin = expand ? 16 : -16;
            Vector2 positionMargin = new Vector2(0, margin);
            Vector2 sizeMargin = new Vector2(0, margin * 2);

            _myRect.IncreaseSizeAndPos(positionMargin, sizeMargin);
            _backgroundRect.IncreaseSizeAndPos(positionMargin, sizeMargin);
            _chatRect.IncreaseSizeAndPos(positionMargin * 2, Vector2.zero);
        }


        public void Refresh()
        {
            SetUpChat(true);
            ChatRefresh.SetActive(false);
        }

        #endregion

        /// <summary>
        /// Initializes chat client and connect to chat room
        /// </summary>
        public void InitializeChatConnection()
        {
            // Get chat region
            List<string> regionNames = Enum.GetNames(typeof(Globals.Enums.ChatRegionCode)).ToList();
            int regionIndex = PlayerPrefs.GetInt(Globals.PUNKeys.chatRegion);

            // Set channel name to game room name and userid to player name
            _channelName = PlayerPrefs.GetString(Globals.PUNKeys.gameRoomName);
            _userId = PlayerPrefs.GetString(Globals.PUNKeys.playerName);

            // Initialize chat client
            _chatClient = new ChatClient(this);
            _chatClient.ChatRegion = regionNames[regionIndex].ToUpper();
            _chatClient.Connect(
                Globals.PUNAppIds.chat,
                Globals.PUNVersion,
                new ExitGames.Client.Photon.Chat.AuthenticationValues(_userId));
            Application.runInBackground = true;

            _connected = true;
            // Start serving messages
            // StartCoroutine("ServeMessages");
        }

        /// <summary>
        /// Send text to other users
        /// </summary>
        public void SendText(string text)
        {
            if (_subscribedToRoom == false)
            {
                Debug.Log("Error. Cannot send msg because not subscribed to room");
                return;
            }

            _myInputField.text = string.Empty;
            if(Regex.Match(text, @"^\s*$").Success)
            {
                return;
            }

            _chatClient.PublishMessage(_channelName, text);
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            string fullMsg = string.Empty;
            foreach (string msg in messages)
                fullMsg += msg;

            CreateMessage(fullMsg, senders[0], senders.Contains(_userId));
        }

        private void CreateMessage(string text, string sender, bool myMessage)
        {
            GameObject message = Instantiate(ChatMessage,
                ChatMessage.transform.position,
                Quaternion.identity,
                _chatContent.transform);

            SetUpMessage(message, sender, text, myMessage);
        }

        /// <summary>
        /// Set component text to formatted message
        /// </summary>
        private void SetUpMessage(GameObject message, string sender, string text, bool myMessage)
        {
            Text msgText = message.GetComponent<Text>();
            if (msgText == null)
                throw new Exception("Error. Message doesn't have text field");

            if (myMessage)
            {
                msgText.color = Color.blue;
                msgText.text = String.Format("You: {0}", text);
            }
            else
            {
                msgText.color = Color.black;
                msgText.text = String.Format("{0}: {1}", sender, text);
            }
            message.SetActive(true);

            AppendMessageOnOthers(message, (int)msgText.preferredHeight);
        }

        /// <summary>
        /// Method append new message to list and moves other messages to top
        /// </summary>
        /// <param name="margin">Message preffered height</param>
        private void AppendMessageOnOthers(GameObject message, int margin)
        {
            if (_chatMessages.Count > 0)
            {
                foreach (GameObject chatMsg in _chatMessages)
                {
                    chatMsg.transform.position += new Vector3(0, margin, 0);
                }
            }
            // Use anchored position because Chat Message transform position doesn't exist yet
            // Lift vertical position by half of message size
            message.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, margin * 0.5f);
            _chatMessages.Add(message);

            if (_chatMessages.Count == MAX_MSG_IN_SCREEN)
            {
                GameObject firstMessage = _chatMessages[0];
                _chatMessages.Remove(firstMessage);
                Destroy(firstMessage);
            }
        }

        /// <summary>
        /// Method which set chat by supplied state
        /// </summary>
        /// <param name="enabled"></param>
        public void SetUpChat(bool enabled)
        {
            if (_chatClient == null && !enabled)
                return;

            SetUpChatColor(enabled);
            if(enabled)
            {
                StopAllCoroutines();
                InitializeChatConnection();
            }
            else
            {
                _myInputField.interactable = false;
                _myInputField.placeholder.gameObject.SetActive(true);
                _chatClient.Disconnect();
            }
        }

        public void SetUpChatColor(bool enabled)
        {
            Color enabledColor = enabled ? Color.white : new Color(0.447f, 0.447f, 0.447f, 0.5f);
            ScrollViewImage.color = enabledColor;
            _myImage.color = enabledColor;
        }

        public void OnApplicationQuit()
        {
            if (_chatClient != null) _chatClient.Disconnect();
            PhotonNetwork.Disconnect();
        }
    }
}