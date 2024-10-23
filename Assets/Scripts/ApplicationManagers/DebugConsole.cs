﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;
using System.Diagnostics;
using GameManagers;

namespace ApplicationManagers
{
    /// <summary>
    /// A GUI console used by testers to view debug logs and type debug commands.
    /// Can be activated by pressing F11.
    /// See DebugTesting for debug command implementations.
    /// </summary>
    class DebugConsole : MonoBehaviour
    {
        static DebugConsole _instance;
        public static bool Enabled;
        static LinkedList<string> _messages = new LinkedList<string>();
        static int _currentCharCount = 0;
        static Vector2 _scrollPosition = Vector2.zero;
        static string _inputLine = string.Empty;
        static bool _needResetScroll;
        const int MaxMessages = 200;
        const int MaxChars = 5000;
        const int MaxCharsPerMessage = 1000;
        const int PositionX = 20;
        const int PositionY = 20;
        const int Width = 400;
        const int Height = 300;
        const int InputHeight = 25;
        const int Padding = 10;
        const string InputControlName = "DebugInput";

        public static void Init()
        {
            _instance = SingletonFactory.CreateSingleton(_instance);
            Application.RegisterLogCallback(OnUnityDebugLog);
        }

        public static void Log(string message, bool showInChat = false)
        {
            UnityEngine.Debug.Log(message);
            if (showInChat && ChatManager.IsChatAvailable())
                ChatManager.AddException(message);
        }

        public static void DebugLog(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogTimeSince(float start, string prefix = "")
        {
            UnityEngine.Debug.Log(prefix + ": " + (Time.realtimeSinceStartup - start).ToString());
        }

        static void OnUnityDebugLog(string log, string stackTrace, LogType type)
        {
            AddMessage(stackTrace);
            AddMessage(log);
        }

        static void AddMessage(string message)
        {
            while (message.Length > MaxCharsPerMessage)
            {
                AddMessage(message.Substring(0, MaxCharsPerMessage));
                message = message.Substring(MaxCharsPerMessage);
            }
            _messages.AddLast(message);
            _currentCharCount += message.Length;
            while (_messages.Count > MaxMessages || _currentCharCount > MaxChars)
            {
                _currentCharCount -= _messages.First.Value.Length;
                _messages.RemoveFirst();
            }
            _needResetScroll = true;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
                Enabled = !Enabled;
        }

        void OnGUI()
        {
            if (Enabled)
            {
                // draw debug console over everything else
                GUI.depth = 1;
                GUI.Box(new Rect(PositionX, PositionY, Width, Height), "");
                DrawMessageWindow();
                DrawInputWindow();
                HandleInput();
                GUI.depth = 0;
            }
        }

        static void DrawMessageWindow()
        {
            int positionX = PositionX + Padding;
            int positionY = PositionY + Padding;
            int width = Width - Padding * 2;
            GUI.Label(new Rect(positionX, positionY, width, InputHeight), "Debug Console (Press F11 to hide)");
            positionY += InputHeight + Padding;
            int scrollViewHeight = Height - Padding * 4 - InputHeight * 2;
            GUIStyle style = new GUIStyle(GUI.skin.box);
            string text = "";
            foreach (string message in _messages)
                text += message + "\n";
            int textWidth = width - Padding * 2;
            int height = (int)style.CalcHeight(new GUIContent(text), textWidth) + Padding;
            _scrollPosition = GUI.BeginScrollView(new Rect(positionX, positionY, width, scrollViewHeight), _scrollPosition,
                new Rect(positionX, positionY, textWidth, height));
            GUI.Label(new Rect(positionX, positionY, textWidth, height), text);
            if (_needResetScroll)
            {
                _needResetScroll = false;
                _scrollPosition = new Vector2(0f, height);
            }
            GUI.EndScrollView();
        }

        static void DrawInputWindow()
        {
            int y = PositionY + Height - InputHeight - Padding;
            GUI.SetNextControlName(InputControlName);
            _inputLine = GUI.TextField(new Rect(PositionX + Padding, y, Width - Padding * 2, InputHeight), _inputLine);
        }

        static void HandleInput()
        {
            if (GUI.GetNameOfFocusedControl() == InputControlName)
            {
                if (IsEnterUp())
                {
                    if (_inputLine != string.Empty)
                    {
                        UnityEngine.Debug.Log(_inputLine);
                        if (_inputLine.StartsWith("/"))
                            DebugTesting.RunDebugCommand(_inputLine.Substring(1));
                        else
                            UnityEngine.Debug.Log("Invalid debug command.");
                        _inputLine = string.Empty;
                    }
                    GUI.FocusControl(string.Empty);
                }
            }
            else if (IsEnterUp())
            {
                GUI.FocusControl(InputControlName);
            }
        }

        static bool IsEnterUp()
        {
            return Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
        }
    }
}
