using System;
using System.Collections.Generic;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public static class Globals
    {
        public static partial class Names
        {
            public static string[] PlayerPrefs = new string[] {
                Globals.PUNKeys.playerName,
                Globals.PUNKeys.gameRoomName,
                Globals.PUNKeys.cloudRegion,
                Globals.PUNKeys.chatRegion
            };

            public static string[] NetworkButtons = new string[] {
                "Connect",
                "Disconnect",
                "Host",
                "Join",
                "StartGame",
                "Return",
                "Exit"
            };

            public static string[] NetworkInputs = new string[] {
                "PlayerName",
                "RoomName",
                "CloudRegion",
                "ChatRegion"
            };

            public static string[] NetworkTexts = new string[] {
                "RequiredToConnect",
                "CurrentlyConnected",
                "NetworkState",
                "ErrorState"
            };


            public static string[] CountDowns = new string[]
            {
                "WaitingForPlayer",
                "3",
                "2",
                "1",
                "Start"
            };
        }

        public static class Enums
        {
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

            public enum NetworkButtons
            {
                Connect,
                Disconnect,
                Host,
                Join,
                StartGame,
                Return,
                Exit
            }

            public enum NetworkInputs
            {
                PlayerName,
                RoomName,
                CloudRegion,
                ChatRegion
            }

            public enum NetworkTexts
            {
                RequiredToConnect,
                CurrentlyConnected,
                NetworkState,
                ErrorState
            }

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
        }


        public static string PUNVersion = "1.0";

        public static partial class PUNAppIds
        {
            public const string chat = "29f6c1e9-d555-43c0-bbf7-ce05ad05cbbf";
            public const string network = "24d3a1f4-57b0-46d1-8e62-a96a1aa64df8";
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
            public const string GameController = "GameController";
            public const string NetworkManager = "NetworkManager";
            public const string NetworkInputs = "NetworkInputs";
            public const string NetworkButtons = "NetworkButtons";
            public const string NetworkTexts = "NetworkTexts";
            public const string ChatContent = "ChatContent";
            public const string GUIBackground = "GUIBackground";
            public const string CountDownObjs = "CountDownObjs";

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
            new Color(0, 0, 0, 0),
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
            {25, KeyCode.M },
            {26, KeyCode.LeftArrow },
            {27, KeyCode.UpArrow },
            {28, KeyCode.RightArrow },
            {29, KeyCode.KeypadMinus},
            {30, KeyCode.Space }
        };

        public const int SpaceKeyIndex = 30;

        public static partial class NetworkData
        {
            public const bool Offline_Mode = true;
        }

        public static partial class Methods
        {
            /// <typeparam name="T"></typeparam>
            /// <param name="UIComponents">Array with fixed length</param>
            /// <param name="names">names for gameobject to check</param>
            /// <param name="tag">tag </param>
            public static void SetupUI<T>(ref T[] UIComponents, string[] names, string tag)
            {
                GameObject[] UIObjects = GameObject.FindGameObjectsWithTag(tag);
                UIComponents = new T[UIObjects.Length];
                for (int i = 0; i < UIObjects.Length; i++)
                {
                    UIComponents[i] = UIObjects[i].GetComponent<T>();
                }

                SortByName<T>(UIObjects, names, UIComponents);
            }

            /// <summary>
            /// Setup GameObjects getting by tags and sorting by names
            /// </summary>
            public static void SetupGOs(ref GameObject[] GameObjects, string[] names, string tag)
            {
                GameObjects = GameObject.FindGameObjectsWithTag(tag);
                SortByName<GameObject>(GameObjects, names);
            }

            public static void SortByName<T>(GameObject[] sortedObjects, string[] names, T[] additionalObjects = null)
            {
                // Sort by names
                for (int i = 0; i < sortedObjects.Length - 1; i++)
                {
                    int j = i;
                    while (sortedObjects[i].name != names[i])
                    {
                        j++;
                        if (sortedObjects.Length <= j)
                        {
                            throw new Exception("Cannot find name " + names[i]);
                        }
                        Swap(sortedObjects, i, j);
                        if (additionalObjects != null)
                            Swap(additionalObjects, i, j);
                    }
                }
            }

            public static void Swap<T>(T[] objects, int i, int j)
            {
                T temp = objects[i];
                objects[i] = objects[j];
                objects[j] = temp;
            }
        }

        public static string[] CurseWords = new string[]
        {
              "Alcoholic",
              "Amateur",
              "Analphabet",
              "Anarchist",
              "Ape",
              "Die",
              "Arse",
              "Arselicker",
              "Ass",
              "Ass master",
              "Ass-kisser",
              "Ass-nugget",
              "Ass-wipe",
              "Asshole",
              "Baby",
              "Backwoodsman",
              "Balls",
              "Bandit",
              "Barbar",
              "Bastard",
              "Bastard",
              "Beavis",
              "Beginner",
              "Biest",
              "Bitch",
              "Blubber gut",
              "Bogeyman",
              "Booby",
              "Boozer",
              "Bozo",
              "Brain-fart",
              "Brainless",
              "Brainy",
              "Brontosaurus",
              "Brownie",
              "Bugger",
              "Bugger, silly",
              "Bulloks",
              "Bum",
              "Bum-fucker",
              "Butt",
              "Buttfucker",
              "Butthead",
              "Callboy",
              "Callgirl",
              "Camel",
              "Cannibal",
              "Cave man",
              "Chaavanist",
              "Chaot",
              "Chauvi",
              "Cheater",
              "Chicken",
              "Children fucker",
              "Clit",
              "Clown",
              "Cock",
              "Cock master",
              "Cock up",
              "Cockboy",
              "Cockfucker",
              "Cockroach",
              "Coky",
              "Con merchant",
              "Con-man",
              "Country bumpkin",
              "Cow",
              "Creep",
              "Creep",
              "Cretin",
              "Criminal",
              "Cunt",
              "Cunt sucker",
              "Daywalker",
              "Deathlord",
              "Derr brain",
              "Desperado",
              "Devil",
              "Dickhead",
              "Dinosaur",
              "Disguesting packet",
              "Diz brain",
              "Do-Do",
              "Dog",
              "Dog, dirty",
              "Dogshit",
              "Donkey",
              "Drakula",
              "Dreamer",
              "Drinker",
              "Drunkard",
              "Dufus",
              "Dulles",
              "Dumbo",
              "Dummy",
              "Dumpy",
              "Egoist",
              "Eunuch",
              "Exhibitionist",
              "Fake",
              "Fanny",
              "Farmer",
              "Fart",
              "Fart, shitty",
              "Fatso",
              "Fellow",
              "Fibber",
              "Fish",
              "Fixer",
              "Flake",
              "Flash Harry",
              "Freak",
              "Frog",
              "Fuck",
              "Fuck face",
              "Fuck head",
              "Fuck noggin",
              "Fucker",
              "Gangster",
              "Ghost",
              "Goose",
              "Gorilla",
              "Grouch",
              "Grumpy",
              "Head, fat",
              "Hell dog",
              "Hillbilly",
              "Hippie",
              "Homo",
              "Homosexual",
              "Hooligan",
              "Horse fucker",
              "Idiot",
              "Ignoramus",
              "Jack-ass",
              "Jerk",
              "Joker",
              "Junkey",
              "Killer",
              "Lard face",
              "Latchkey child",
              "Learner",
              "Liar",
              "Looser",
              "Lucky",
              "Lumpy",
              "Luzifer",
              "Macho",
              "Macker",
              "Man, old",
              "Minx",
              "Missing link",
              "Monkey",
              "Monster",
              "Motherfucker",
              "Mucky pub",
              "Mutant",
              "Neanderthal",
              "Nerfhearder",
              "Nobody",
              "Nurd",
              "Nuts, numb",
              "Oddball",
              "Oger",
              "Oil dick",
              "Old fart",
              "Orang-Uthan",
              "Original",
              "Outlaw",
              "Pack",
              "Pain in the ass",
              "Pavian",
              "Pencil dick",
              "Pervert",
              "Pig",
              "Piggy-wiggy",
              "Pirate",
              "Pornofreak",
              "Prick",
              "Prolet",
              "Queer",
              "Querulant",
              "Rat",
              "Rat-fink",
              "Reject",
              "Retard",
              "Riff-Raff",
              "Ripper",
              "Roboter",
              "Rowdy",
              "Rufian",
              "Sack",
              "Sadist",
              "Saprophyt",
              "Satan",
              "Scarab",
              "Schfincter",
              "Shark",
              "Shit eater",
              "Shithead",
              "Simulant",
              "Skunk",
              "Skuz bag",
              "Slave",
              "Sleeze",
              "Sleeze bag",
              "Slimer",
              "Slimy bastard",
              "Small pricked",
              "Snail",
              "Snake",
              "Snob",
              "Snot",
              "Son of a bitch ",
              "Square",
              "Stinker",
              "Stripper",
              "Stunk",
              "Swindler",
              "Swine",
              "Teletubby",
              "Thief",
              "Toilett cleaner",
              "Tussi",
              "Typ",
              "Unlike",
              "Vampir",
              "Vandale",
              "Varmit",
              "Wallflower",
              "Wanker",
              "Wanker, bloody",
              "Weeze Bag",
              "Whore",
              "Wierdo",
              "Wino",
              "Witch",
              "Womanizer",
              "Woody allen",
              "Worm",
              "Xena",
              "Xenophebe",
              "Xenophobe",
              "XXX Watcher",
              "Yak",
              "Yeti",
              "Zit face"
        };
    }
}