/*
 * Author: Bharath Kumar S
 * Date: 2-10-2024
 * Description: Lobby logic
 */
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class KitchenGameLobby : MonoBehaviour {
    public static KitchenGameLobby Instance { get; private set; }
    private Lobby joinedLobby;
    private float heartBeatTimer;
    private float listLobbiesTimer;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChnagedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChnagedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }
    private async void InitializeUnityAuthentication() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {

            //to test with multiple builds 
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async void FetchAllLobies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChnagedEventArgs {
                lobbyList = queryResponse.Results,
            });
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    public async void CreatLobby(string lobbyName, bool isPrivate) {
        try {
            OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYERS_ALLOWED, new CreateLobbyOptions {
                IsPrivate = isPrivate,
            });
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e) {
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }
    private void Update() {
        HandleHeartBeat();
        HandlePeriodicListLobbies();
    }
    private void HandlePeriodicListLobbies() {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn) {
            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer < 0f) {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                FetchAllLobies();
            }
        }
    }
    private void HandleHeartBeat() {
        if (IsLobbyHost()) {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f) {
                float heartBeatTimerMax = 15f;
                heartBeatTimer = heartBeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    public bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    public async void QuickJoin() {
        try {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }
    public async void JoinWithCode(string lobbyCode) {
        try {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }
    public async void JoinWithId(string lobbyId) {
        try {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }
    public Lobby GetLobby() {
        return joinedLobby;
    }
    public async void DeleteLobby() {
        try {
            if (joinedLobby != null) {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            }
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public async void KickLobby(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
}