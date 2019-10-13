using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    [RequireComponent(typeof(Animator))]
    public class ChatController : MonoBehaviour, IChatClientListener
    {

        #region publicVars
        public GameObject ChatMessage;
        public TMP_InputField ChatInputField;
        public TextMeshProUGUI ChatInput;
        public GameObject ChatContent = null;
        public Color ColorOfMyMessage = Color.blue;
        public Color ColorOfOtherMessage = Color.black;

        public bool IsChatConnected { get { return _connected; } }

        #endregion publicVars

        #region privateVars

        private List<GameObject> _chatMessages = new List<GameObject>();

        private ChatClient _chatClient = null;
        private Animator _chatAnimator;
        private GameObject _lastMessage = null;

        private string _channelName = null;
        private string _userId = null;


        private bool _subscribedToRoom = false;
        private bool _connected = false;
        private bool _chatExpanded = false;

        const int MAX_MSG_IN_SCREEN = 25;
        const int OVERFLOW_SIZE = 28;

        #endregion privateVars

        // Use this for initialization because after Network Manager Start method some content won't be available
        void Awake()
        {
            // Asure objects are ready
            if (ChatMessage == null)
            {
                throw new Exception("Error. Chat message object not supplied");
            }
            
            if (ChatInputField == null)
            {
                throw new Exception("Error. Chat input field couldn't be found");
            }

            if (ChatInput== null)
            {
                throw new Exception("Error. Chat input couldn't be found");
            }

            if (ChatContent == null)
            {
                throw new Exception("Error. Chat content couldn't be found");
            }

            GameObject networkManagerObject = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkManager);
            if (networkManagerObject == null)
            {
                throw new Exception("Error. No Network Manager in Scene");
            }

            _chatAnimator = GetComponent<Animator>();
            ChatInput.color = ColorOfMyMessage;
        }

        void Update()
        {
            if (_connected)
            {
                _chatClient.Service();
            }
        }

        #region ChatClientInterfaceProps

        public void OnConnected()
        {
            Debug.Log("Connected");

            _chatClient.Subscribe(new[] { _channelName });
            _chatClient.SetOnlineStatus(ChatUserStatus.Online);

        }

        public void OnDisconnected()
        {
            Debug.Log("Disonnected: " + _chatClient.DisconnectedCause.ToString());

            _connected = false;
            StopAllCoroutines();

        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("OnChatStateChange");
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log("Private msg: " + message);

        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            string fullMsg = string.Empty;
            foreach (string msg in messages)
                fullMsg += msg;

            CreateMessage(fullMsg, senders[0], senders.Contains(_userId));
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log("status update: " + user + ' ' + status.ToString() + ' ' + message.ToString());
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log("DebugReturn");
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            _connected = true;
            _chatAnimator.SetBool(Globals.ChatAnimatorKeys.Enabled, true);

        }

        public void OnUnsubscribed(string[] channels)
        {
            _connected = false;
            _chatAnimator.SetBool(Globals.ChatAnimatorKeys.Enabled, false);
        }

        #endregion ChatClientInterfaceProps

        // /// <summary>
        // /// Initializes chat client and connect to chat room
        // /// </summary>
        public void InitializeChatConnection()
        {
            // Get chat region
            // List<string> regionNames = Enum.GetNames(typeof(Globals.Enums.ChatRegionCode)).ToList();
            // int regionIndex = PlayerPrefs.GetInt(Globals.PUNKeys.chatRegion);

            // Set channel name to game room name and userid to player name
            _channelName = PlayerPrefs.GetString(Globals.PUNKeys.gameRoomName);
            _userId = PlayerPrefs.GetString(Globals.PUNKeys.userId);

            // Initialize chat client
            _chatClient = new ChatClient(this);
            _chatClient.ChatRegion = "EU";
            _chatClient.Connect(
                Globals.PUNAppIds.chat,
                Globals.PUNVersion,
                new AuthenticationValues(_userId));
            Application.runInBackground = true;

            _connected = true;
        }

        /// <summary>
        /// Send text to other users
        /// </summary>
        public void SendText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return;
            }

            ChatInputField.text = "";
            _chatClient.PublishMessage(_channelName, text);
        }

        private void CreateMessage(string text, string sender, bool myMessage)
        {
            GameObject message = Instantiate(ChatMessage,
                ChatMessage.transform.position,
                Quaternion.identity,
                ChatContent.transform);

            SetUpMessage(message, sender, text, myMessage);
        }

        /// <summary>
        /// Set component text to formatted message
        /// </summary>
        private void SetUpMessage(GameObject message, string sender, string text, bool myMessage)

        {
            TextMeshProUGUI textComponent = message.GetComponent<TextMeshProUGUI>();
            if (!textComponent)
            {
                throw new Exception("Error. Message doesn't have text field");
            }
            if (myMessage)
            {
                textComponent.color = ColorOfMyMessage;
                textComponent.text = String.Format("You: {0}", text);
            }
            else
            {
                textComponent.color = ColorOfOtherMessage;
                textComponent.text = String.Format("{0}: {1}", sender, text);
            }

            message.transform.SetAsFirstSibling();
        }

        public void OnApplicationQuit()
        {
            if (_chatClient != null) _chatClient.Disconnect();
            PhotonNetwork.Disconnect();
        }

        public void OnUserSubscribed(string channel, string user)
        {
            throw new NotImplementedException();
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            throw new NotImplementedException();
        }
    }
}