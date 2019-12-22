using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public static class ChannelNames
    {
        public static string Global = "Global";
    }

    public enum MessageTypes
    {
        OwnerMessage = 0,
        OthersMessage = 1,
        SystemMessage = 2
    }

    [RequireComponent(typeof(Animator))]
    public class ChatController : MonoBehaviour, IChatClientListener
    {

        #region publicVars
        public GameObject ChatMessage;
        public TMP_InputField ChatInputField;
        public TextMeshProUGUI ChatInput;
        public GameObject ChatContent = null;
        public bool IsChatConnected { get { return _connected; } }

        private Color OwnerMsgColor = new Color(0.7f, 0.22f, 0.2f);
        private Color OthersMsgColor = new Color(0.15f, 0.46f, 0.87f);
        
        public Color SystemMsgColor = new Color(0.15f, 0.15f, 0.15f);


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
            _chatAnimator.SetTrigger(Globals.SidePaneBtnAnimTriggers.Disabled);
            ChatInput.color = OwnerMsgColor;
            InitializeChatConnection();
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

            _channelName = ChannelNames.Global;
            _chatClient.Subscribe(new[] { _channelName });
            _chatClient.SetOnlineStatus(ChatUserStatus.Online);
            _chatAnimator.SetTrigger(Globals.SidePaneBtnAnimTriggers.Normal);
            CreateMessage(
                (
                    "These are commands for chat\n" + 
                    "Shift + Tab - You can change from private to global channel\n" + 
                    "\\" + "to friendName - message to friend in private\n" +
                    "\\help - shows available commands."
                ),
                "",
                (int)MessageTypes.SystemMessage
            );

        }

        public void OnDisconnected()
        {
            Debug.Log("Disonnected: " + _chatClient.DisconnectedCause.ToString());

            _connected = false;
            _chatAnimator.SetTrigger(Globals.SidePaneBtnAnimTriggers.Disabled);
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

            CreateMessage(fullMsg, senders[0], senders.Contains(_userId) ? 0 : 1);
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
            _chatAnimator.SetTrigger(Globals.SidePaneBtnAnimTriggers.Normal);
        }

        public void OnUnsubscribed(string[] channels)
        {
            _connected = false;
            _chatAnimator.SetTrigger(Globals.SidePaneBtnAnimTriggers.Disabled);
        }

        #endregion ChatClientInterfaceProps

        // /// <summary>
        // /// Initializes chat client and connect to chat room
        // /// </summary>
        public void InitializeChatConnection()
        {
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

        private void CreateMessage(string text, string sender, int messageType)
        {
            GameObject message = Instantiate(ChatMessage,
                ChatMessage.transform.position,
                Quaternion.identity,
                ChatContent.transform);

            SetUpMessage(message, sender, text, messageType);
        }

        /// <summary>
        /// Set component text to formatted message
        /// </summary>
        private void SetUpMessage(GameObject message, string sender, string text, int messageType)

        {
            TextMeshProUGUI textComponent = message.GetComponent<TextMeshProUGUI>();
            if (!textComponent)
            {
                throw new Exception("Error. Message doesn't have text field");
            }

            switch(messageType)
            {
                case (int)MessageTypes.OwnerMessage:
                    textComponent.color = new Color(0.7f, 0.22f, 0.2f);
                    textComponent.text = String.Format("You: {0}", text);
                    break;
                case (int)MessageTypes.OthersMessage:
                    textComponent.color = new Color(0.15f, 0.46f, 0.87f);
                    textComponent.text = String.Format("{0}: {1}", sender, text);
                    break;
                case (int)MessageTypes.SystemMessage:
                    textComponent.color = new Color(0.1f, 0.1f, 0.1f);
                    textComponent.text = text;
                    break;
                default:
                    break;
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