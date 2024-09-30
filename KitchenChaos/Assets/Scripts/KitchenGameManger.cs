/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Game Manger 
 */

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public enum State {
    WaitingToStart,
    CoolDownToStart,
    GamePlaying,
    GameOver,
}
public class KitchenGameManger : NetworkBehaviour {
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiPlayerGamePaused;
    public event EventHandler OnMultiPlayerGameUnPaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    private bool localPlayerReady = false;
    public static KitchenGameManger Instance { get; private set; }
    private NetworkVariable<float> countDownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 300f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePausedState = false;
    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChange;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        autoTestGamePausedState = true;
    }
    private void IsGamePaused_OnValueChange(bool wasPaused, bool goingToBePaused) {
        if (isGamePaused.Value) {
            Time.timeScale = 0f;
            OnMultiPlayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnMultiPlayerGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
    private void State_OnValueChanged(State oldState, State newSate) {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    private void Awake() {
        playerReadyDictionary = new Dictionary<ulong, bool>();
        Instance = this;
    }
    private void Start() {
        GameInput.Instance.OnPauseButtonClicked += GameInput_OnPauseButtonCLicked;
        GameInput.Instance.OnInteractionPerformed += GameInput_OnInteractionPerformed;
    }
    private void GameInput_OnInteractionPerformed(object sender, EventArgs e) {
        if (state.Value == State.WaitingToStart) {
            localPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsAreReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerReadyDictionary[clientId] || playerReadyDictionary.ContainsKey(clientId)) {
                //some player not ready
                allClientsAreReady = false;
                break;
            }
        }
        if (allClientsAreReady) {
            state.Value = State.CoolDownToStart;
        }
    }
    private void GameInput_OnPauseButtonCLicked(object sender, EventArgs e) {
        TogglePauseGame();
    }
    private void Update() {
        if (!IsOwner) {
            return;
        }
        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CoolDownToStart:
                countDownToStartTimer.Value -= Time.deltaTime;
                if (countDownToStartTimer.Value < 0f) {
                    state.Value = State.GamePlaying;
                }
                gamePlayingTimer.Value = gamePlayingTimerMax;

                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f) {
                    state.Value = State.GameOver;
                }

                break;
            case State.GameOver:
                //do nothing
                break;
            default: break;
        }
    }
    private void LateUpdate() {

        if (autoTestGamePausedState) {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }
    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }
    public bool IsWaitingToStart() {
        return state.Value == State.WaitingToStart;
    }
    public bool IsInCountDownState() {
        return state.Value == State.CoolDownToStart;
    }
    public float GetCountDownTime() {
        return countDownToStartTimer.Value;
    }
    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }
    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public void TogglePauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) {
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
            PauseGameServerRpc();
        }
        else {
            OnLocalGameUnPaused?.Invoke(this, EventArgs.Empty);
            UnPauseGameServerRpc();
        }
    }
    public bool GetIfLocalPlayerRedy() {
        return localPlayerReady;
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }

    private void TestGamePausedState() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerPausedDictionary.ContainsKey(clientId) || playerPausedDictionary[clientId]) {
                //this plaer is paused
                isGamePaused.Value = true;
                return;
            }
        }
        //all players are unpaused
        isGamePaused.Value = false;
    }
}
