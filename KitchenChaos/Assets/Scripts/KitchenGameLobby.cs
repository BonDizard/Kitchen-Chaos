/*
 * Author: Bharath Kumar S
 * Date: 2-10-2024
 * Description: Lobby logic
 */
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class KitchenGameLobby : MonoBehaviour {
    public static KitchenGameLobby Instance { get; private set; }
    private Lobby joinedLobby;
    private float heartBeatTimer;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }
    private async void InitializeUnityAuthentication() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {

            //to test with multiple builds 
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async void CreatLobby(string lobbyName, bool isPrivate) {
        try {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYERS_ALLOWED, new CreateLobbyOptions {
                IsPrivate = isPrivate,
            });
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    private void Update() {
        HandleHeartBeat();
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
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    public async void JoinWithCode(string lobbyCode) {
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    public Lobby GetLobby() {
        return joinedLobby;
    }
}