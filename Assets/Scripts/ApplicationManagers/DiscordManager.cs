using Photon.Pun;
using Photon.Realtime;
using Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using Utility;
using Discord.Sdk;

namespace ApplicationManagers
{
    class DiscordManager : MonoBehaviour
    {
        private static DiscordManager _instance;
        private ulong clientID = 480451799908876289;

        private Client client;
        
        private string codeVerifier;

        public static void Init()
        {
            _instance = SingletonFactory.CreateSingleton(_instance);
            _instance.InitDiscord();
        }

        private void InitDiscord()
        {
            client = new Client();
            
            client.AddLogCallback(OnLog, LoggingSeverity.Error);
            client.SetStatusChangedCallback(OnStatusChanged);
        }
        
        public static void StartOAuth()
        {
            if (_instance != null)
            {
                _instance.StartOAuthFlow();
            }
        }
        
        private void OnLog(string message, LoggingSeverity severity)
        {
            Debug.Log($"Log: {severity}: {message}");
        }

        private void OnStatusChanged(Client.Status status, Client.Error error, int errorCode)
        {
            Debug.Log($"Status Changed: {status}");
            if (error != Client.Error.None)
            {
                Debug.LogError($"Error: {error}, Code: {errorCode}");
            }
        }
        
        private void StartOAuthFlow() {
            var authorizationVerifier = client.CreateAuthorizationCodeVerifier();
            codeVerifier = authorizationVerifier.Verifier();
        
            var args = new AuthorizationArgs();
            args.SetClientId(clientID);
            args.SetScopes(Client.GetDefaultPresenceScopes());
            args.SetCodeChallenge(authorizationVerifier.Challenge());
            client.Authorize(args, OnAuthorizeResult);
        }
        
        private void OnAuthorizeResult(ClientResult result, string code, string redirectUri) {
            Debug.Log($"Authorization result: [{result.Error()}] [{code}] [{redirectUri}]");
            if (!result.Successful()) {
                return;
            }
            GetTokenFromCode(code, redirectUri);
        }

        private void GetTokenFromCode(string code, string redirectUri) {
            client.GetToken(clientID,
                code,
                codeVerifier,
                redirectUri,
                (result, token, refreshToken, tokenType, expiresIn, scope) =>
                {
                    Debug.Log($"Token: {token}");
                });
        }
        
        

    }
    
    
}