using System.Collections.Generic;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public static class Globals
    {
        public static bool MyTurn = false;

        public enum SceneIndex
        {
            MainMenu = 0,
            Credits = 1,
            Lobby = 2,
            Game = 3
        }

        public enum PlayerPrefFlags
        {
            PlayerName,
            GameRoomName,
            CloudRegion,
            ChatRegion
        }

        public enum NetworkButtonsEnum
        {
            Connect,
            Disconnect,
            Host,
            Join,
            StartGame,
            Return,
            Exit
        }

        public enum NetworkInputsEnum
        {
            PlayerName,
            RoomName,
            CloudRegion,
            ChatRegion
        }

        public enum NetworkTextEnum
        {
            RequiredToConnect,
            CurrentlyConnected,
            NetworkState,
            ErrorState
        }

        public static string[] PlayerPrefs = new string[] {
            Globals.PUNKeys.playerName,
            Globals.PUNKeys.gameRoomName,
            Globals.PUNKeys.cloudRegion,
            Globals.PUNKeys.chatRegion
        };

        public enum ChatRegionCode
        {
            asia,
            eu,
            us
        }

        public enum RegionType
        {
            Cloud,
            Chat
        }

        public static string PUNVersion = "1.0";

        public static partial class PUNAppIds
        {
            public const string chat = "29f6c1e9-d555-43c0-bbf7-ce05ad05cbbf";
        }

        public static partial class PUNKeys
        {
            public const string playerName = "playerName";
            public const string gameRoomName = "roomName";
            public const string cloudRegion = "cloudRegion";
            public const string chatRegion = "chatRegion";
        }

        public static partial class Tags
        {
            public const string GameEndGUI = "GameEndGUI";
            public const string NetworkGUI = "NetworkGUI";
            public const string ChatGUI = "ChatGUI";
            public const string KeyGen = "KeyGen";
            public const string Player = "Player";
            public const string Wave = "Wave";
            public const string CountDown = "CountDown";
            public const string GameController = "GameController";
            public const string NetworkManager = "NetworkManager";
            public const string NetworkInputs = "NetworkInputs";
            public const string NetworkButtons = "NetworkButtons";
            public const string NetworkTexts = "NetworkTexts";
            public const string ChatContent = "ChatContent";
            public const string GUIBackground = "GUIBackground";
        }

        public static partial class Defaults
        {
            public const float WaveSpeed = 1.0f;
        }

        public static partial class PrefabNames
        {
            public const string MasterClient = "MasterClient";
            public const string Client = "Client";
        }

        public static partial class Delays
        {
            public const float COUNT_DOWN = 0.5f;
        }

        public partial class Difficulty
        {
            public static int[] DifficultyLevels = new int[]
            {
            3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 9, 9
            };

            public const int MAX_DIFF_LEVEL = 7;
        }

        public static Color[] ColorsByTurn = new Color[]
        {
            Color.white,
            Color.yellow
        };

        public static Color[] InactiveColorsByTurn = new Color[]
        {
            new Color(0.5f, 0.5f, 0.5f, 1),
            new Color(1, 1, 0.3f, 1)
        };

        public enum ColorTurnIndex
        {
            myTurn = 0,
            oponnentTurn = 1
        }

        public static Dictionary<int, KeyCode> keyCodes = new Dictionary<int, KeyCode>()
        {
            {0, KeyCode.Q },
            {1, KeyCode.W },
            {2, KeyCode.E },
            {3, KeyCode.R },
            {4, KeyCode.T },
            {5, KeyCode.Y },
            {6, KeyCode.U },
            {7, KeyCode.I },
            {8, KeyCode.O },
            {9, KeyCode.P },
            {10, KeyCode.A },
            {11, KeyCode.S },
            {12, KeyCode.D },
            {13, KeyCode.F },
            {14, KeyCode.G },
            {15, KeyCode.H },
            {16, KeyCode.J },
            {17, KeyCode.K },
            {18, KeyCode.L },
            {19, KeyCode.Z },
            {20, KeyCode.X },
            {21, KeyCode.C },
            {22, KeyCode.V },
            {23, KeyCode.B },
            {24, KeyCode.N },
            {25, KeyCode.M }
        };
    }
}