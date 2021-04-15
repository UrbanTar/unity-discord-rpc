﻿#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Unity;

namespace ERPC
{
    [InitializeOnLoad]
    public static class ERPC
    {
        public static DiscordRpcClient client;

        public static string projectName;
        public static string sceneName;

        public static string applicationID = "821629735229980702";

        public static string details;
        public static string state;
        public static string largeImageKey = "ab";
        public static string largeImageText = "Arthur Blue";
        public static string smallImageKey = "unity";
        public static string smallImageText = "Made with Unity";
        public static string button1Text = "Itch.io";
        public static string button1Url = "https://tarsier.itch.io/";
        public static string button2Text = "GitHub";
        public static string button2Url = "https://github.com/fenwikk/unity-discord-rpc/";

        public static DateTime start;

        public static bool resetOnSceneChange = false;

        public static DiscordPipe targetPipe = DiscordPipe.FirstAvailable;

        public static LogLevel logLevel = LogLevel.Warning;

        public static double lastEdit;

        /// <summary>
        /// All possible pipes discord can be found on.
        /// </summary>
        public enum DiscordPipe
        {
            FirstAvailable = -1,
            Pipe0 = 0,
            Pipe1 = 1,
            Pipe2 = 2,
            Pipe3 = 3,
            Pipe4 = 4,
            Pipe5 = 5,
            Pipe6 = 6,
            Pipe7 = 7,
            Pipe8 = 8,
            Pipe9 = 9
        }

        static ERPC()
        {
            projectName = Application.productName;
            sceneName = SceneManager.GetActiveScene().name;

            EditorApplication.update += Update;

            start = GetTimeStamp();

            Initialize();
        }

        static void Update()
        {
            if (!EditorApplication.isPlaying == false && client == null)
            {
                Initialize();
            }

            if (sceneName != SceneManager.GetActiveScene().name)
            {
                sceneName = SceneManager.GetActiveScene().name;
                SceneLoaded();
            }

            client.Logger.Level = logLevel;

            projectName = Application.productName;
            sceneName = SceneManager.GetActiveScene().name;

            if (EditorApplication.timeSinceStartup >= lastEdit + 5 && EditorApplication.timeSinceStartup <= lastEdit + 10)
            {
                UpdateActivity();
                
                ERPCWindow.status = "Up to date";

                lastEdit = 0f;
            }
        }

        static void SceneLoaded(Scene scene = new Scene(), LoadSceneMode mode = new LoadSceneMode())
        {
            Debug.Log("Scene Loaded");

            projectName = Application.productName;
            sceneName = SceneManager.GetActiveScene().name;
            start = GetTimeStamp();
            UpdateActivity();
        }

        public static void Initialize()
        {
            Debug.Log("[ERP] Init");

            //Prepare the logger
            DiscordRPC.Logging.ILogger logger = null;

            //Update the logger to the unity logger
            if (Debug.isDebugBuild) logger = new FileLogger("discordrpc.log") { Level = logLevel };
            if (Application.isEditor) logger = new UnityLogger() { Level = logLevel };

            client = new DiscordRpcClient(
                applicationID,                                  //The Discord Application ID            
                pipe: (int)targetPipe,                          //The target pipe to connect too
                // logger: logger,                              //The logger. Will Uncomment after when Lachee fixes issue #136 in Lachee/discord-rpc-csharp
                autoEvents: false,                              //WE will manually invoke events
                client: new UnityNamedPipe()                    //The client for the pipe to use. Unity MUST use a NativeNamedPipeClient since its managed client is broken.
            );
            client.Initialize();                                //Connects the client

            ERPCSettings.GetSettings();
            UpdateActivity();
        }
        public static void UpdateActivity()
        {
            if (!ERPCWindow.customDetailsState)
            {
                details = projectName;
                state = sceneName;
            }

            if (ERPCWindow.button2IsValid)
            {
                client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = largeImageKey,
                        LargeImageText = largeImageText,
                        SmallImageKey = smallImageKey,
                        SmallImageText = smallImageText
                    },
                    Timestamps = new Timestamps
                    {
                        Start = start
                    },
                    Buttons = new Button[]
                    {
                        new Button() { Label = button1Text, Url = button1Url},
                        new Button() { Label = button2Text, Url = button2Url}
                    }
                });
            }
            else if (ERPCWindow.button1IsValid)
            {
                client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = largeImageKey,
                        LargeImageText = largeImageText,
                        SmallImageKey = smallImageKey,
                        SmallImageText = smallImageText
                    },
                    Timestamps = new Timestamps
                    {
                        Start = start
                    },
                    Buttons = new Button[]
                    {
                        new Button() { Label = button1Text, Url = button1Url}
                    }
                });
            }
            else
            {
                client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = largeImageKey,
                        LargeImageText = largeImageText,
                        SmallImageKey = smallImageKey,
                        SmallImageText = smallImageText
                    },
                    Timestamps = new Timestamps
                    {
                        Start = start
                    }
                });
            }

            Debug.Log("[ERP] Updated Presence");

            ERPCSettings.SaveSettings();

            client.Logger.Level = logLevel;
        }

        public static DateTime GetTimeStamp()
        {
            if (!resetOnSceneChange)
            {
                DateTime timeSinceStartup = DateTime.UtcNow.AddSeconds(-EditorApplication.timeSinceStartup);
                return timeSinceStartup;
            }
            DateTime unixTimestamp = DateTime.UtcNow;
            return unixTimestamp;
        }
    }
}
#endif
