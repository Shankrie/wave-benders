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
    [RequireComponent(typeof(InputField))]
    public class ChatController : Photon.MonoBehaviour, IChatClientListener
    {

        #region publicVars
        public GameObject chatMessage = null;
        #endregion publicVars

        #region privateVars

        private List<GameObject> _chatMessages = new List<GameObject>();
        private GameObject _chatContent = null;

        private NetworkManager _networkManager = null;
        private ChatClient _chatClient = null;
        private InputField _myInputField = null;

        private string _channelName = null;
        private string _userId = null;

        private const float dServeTime = 0.1f;
        private const int MSG_MARGIN = 30;
        private const int MAX_MSG_IN_SCREEN = 5;

        private bool _subscribedToRoom = false;
        private bool _triedConnect = false;

        #endregion privateVars

        // Use this for initialization
        void Start()
        {
            // Asure objects are ready
            if (chatMessage == null)
                throw new Exception("Error. Chat message object not supplied");

            GameObject networkManagerObject = GameObject.FindGameObjectWithTag(Globals.Tags.NetworkManager);
            if (networkManagerObject == null)
                throw new Exception("Error. No Network Manager in Scene");

            // get chat content gameobject and input field
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _chatContent = GameObject.FindGameObjectWithTag(Globals.Tags.ChatContent);
            _myInputField = GetComponent<InputField>();
        }

        #region ChatClientInterfaceProps

        public void OnConnected()
        {
            _chatClient.Subscribe(new[] { _channelName });
            _chatClient.SetOnlineStatus(ChatUserStatus.Online);
        }

        public void OnDisconnected()
        {
            Debug.Log("Disonnected: " + _chatClient.DisconnectedCause.ToString());
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
        }

        public void OnApplicationQuit()
        {
            if(_chatClient != null) _chatClient.Disconnect();
            StopAllCoroutines();
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

        private IEnumerator ServeMessages()
        {
            while(true)
            {
                yield return new WaitForSeconds(dServeTime);
                _chatClient.Service();
            }
        }

        #endregion

        public void InitializeChatConnection()
        {
            _triedConnect = true;

            List<string> regionNames = Enum.GetNames(typeof(CloudRegionCode)).ToList();
            int regionIndex = PlayerPrefs.GetInt(Globals.PUNKeys.cloudRegion);

            _channelName = PlayerPrefs.GetString(Globals.PUNKeys.gameRoomName);
            _userId = PlayerPrefs.GetString(Globals.PUNKeys.playerName);

            // initialize chat client
            _chatClient = new ChatClient(this);
            _chatClient.ChatRegion = regionNames[regionIndex];
            _chatClient.Connect(
                Globals.PUNAppIds.chat,
                Globals.PUNVersion,
                new ExitGames.Client.Photon.Chat.AuthenticationValues(_userId));
            Application.runInBackground = true;

            StartCoroutine("ServeMessages");
        }

        public void SendText()
        {
            if (_subscribedToRoom == false)
            {
                Debug.Log("Error. Cannot send msg because not subscribed to room");
                return;
            }

            string text = _myInputField.text;
            _myInputField.text = string.Empty;

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
            GameObject message = Instantiate(chatMessage,
                new Vector3(0, 0, 0),
                Quaternion.identity,
                _chatContent.transform);

            SetUpMessage(message, sender, text, myMessage);
            AppendMessageOnOthers(message);
        }

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
                msgText.text = String.Format("{0}: {1}", sender, text);
            }
            message.SetActive(true);

        }

        public void AppendMessageOnOthers(GameObject message)
        {
            foreach(GameObject chatMsg in _chatMessages)
            {
                chatMsg.transform.position += new Vector3(0, MSG_MARGIN, 0);
            }

            _chatMessages.Add(message);
            if (_chatMessages.Count == MAX_MSG_IN_SCREEN)
            {
                GameObject firstMessage = _chatMessages[0];
                _chatMessages.Remove(firstMessage);
                Destroy(firstMessage);
            }
        }

    }
}