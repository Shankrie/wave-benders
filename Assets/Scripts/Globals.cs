using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Globals
{
    public enum SceneIndex {
        MainMenu = 0,
        Credits = 1,
        Game = 2
    }

    public static partial class PUNKeys
    {
        public const string playerName = "playerName";
        public const string gameRoomName = "gameRoom";
    }

    public static partial class Tags
    {
        public const string NetworkGUI = "NetworkGUI";
        public const string KeyGen = "KeyGen";
        public const string Player = "Player";
        public const string Wave = "Wave";
        public const string CountDown = "CountDown";
        public const string GameEndGUI = "GameEndGUI";
        public const string GameController = "GameController";
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