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
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    private bool localPlayerReady = false;
    public static KitchenGameManger Instance { get; private set; }
    NetworkVariable<float> countDownToStartTimer = new NetworkVariable<float>(3f);
    NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    float gamePlayingTimerMax = 300f;
    bool isGamePaused = false;
    public NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private Dictionary<ulong, bool> playerReadyDictionary;
    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
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
            SetPlayerReadyServerRpc();
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
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

    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
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
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
    public bool GetIfLocalPlayerRedy() {
        return localPlayerReady;
    }
}
