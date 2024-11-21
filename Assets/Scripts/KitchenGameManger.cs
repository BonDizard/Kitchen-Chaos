/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Game Manger 
 */

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameManger : NetworkBehaviour {
    public static KitchenGameManger Instance { get; private set; }


    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    public enum State {
        WaitingToStart,
        CoolDownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<float> countDownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);

    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool localPlayerReady;
    private float gamePlayingTimerMax = 90f;
    private bool isLocalGamePaused = false;
    private bool autoTestGamePausedState;

    private void Awake() {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }
    private void Start() {
        GameInput.Instance.OnPauseButtonClicked += GameInput_OnPauseButtonCLicked;
        GameInput.Instance.OnInteractionPerformed += GameInput_OnInteractionPerformed;
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChange;

        Debug.Log("[KitchenGameManager] Network spawned. Initial state: " + state.Value);

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            Transform playerTranform = Instantiate(playerPrefab);
            playerTranform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
        Debug.Log("[KitchenGameManager] Scene loaded: " + sceneName);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        Debug.LogWarning("[KitchenGameManager] Client disconnected: " + clientId);
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChange(bool wasPaused, bool goingToBePaused) {
        Debug.Log("[KitchenGameManager] Pause state changed. WasPaused: " + wasPaused + ", GoingToBePaused: " + goingToBePaused);
        if (isGamePaused.Value) {
            Time.timeScale = 0f;
            Debug.Log("[KitchenGameManager] Game paused.");
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            Debug.Log("[KitchenGameManager] Game unpaused.");
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State oldState, State newSate) {
        Debug.Log("[KitchenGameManager] State changed from " + oldState + " to " + newSate);
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractionPerformed(object sender, EventArgs e) {
        if (state.Value == State.WaitingToStart) {
            localPlayerReady = true;
            Debug.Log("[KitchenGameManager] Local player ready.");
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        if (!playerReadyDictionary.ContainsKey(senderClientId)) {
            playerReadyDictionary[senderClientId] = false; // Default initialization
            Debug.LogWarning("[KitchenGameManager] Adding default entry for playerReadyDictionary for client: " + senderClientId);
        }

        playerReadyDictionary[senderClientId] = true;
        Debug.Log("[KitchenGameManager] Player ready: " + senderClientId);

        bool allClientsAreReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                allClientsAreReady = false;
                break;
            }
        }

        if (allClientsAreReady) {
            Debug.Log("[KitchenGameManager] All players ready. Transitioning to CoolDownToStart state.");
            state.Value = State.CoolDownToStart;
        }
    }


    private void GameInput_OnPauseButtonCLicked(object sender, EventArgs e) {
        Debug.Log("[KitchenGameManager] Pause button clicked.");
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
                Debug.Log("[KitchenGameManager] Countdown: " + countDownToStartTimer.Value);
                if (countDownToStartTimer.Value < 0f) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                    Debug.Log("[KitchenGameManager] GamePlaying state started. Timer set to: " + gamePlayingTimerMax);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f) {
                    state.Value = State.GameOver;
                    Debug.Log("[KitchenGameManager] Game over. Time ran out.");
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

    public bool IsCountdownToStartActive() {
        return state.Value == State.CoolDownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countDownToStartTimer.Value;
    }

    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }

    public bool IsLocalPlayerReady() {
        return localPlayerReady;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public void TogglePauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        Debug.Log("[KitchenGameManager] Toggling local pause. IsPaused: " + isLocalGamePaused);
        if (isLocalGamePaused) {
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
            PauseGameServerRpc();
        }
        else {
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
            UnPauseGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        if (!playerPausedDictionary.ContainsKey(senderClientId)) {
            playerPausedDictionary[senderClientId] = false; // Default initialization
            Debug.LogWarning("[KitchenGameManager] Adding default entry for playerPausedDictionary for client: " + senderClientId);
        }

        playerPausedDictionary[senderClientId] = true;
        Debug.Log("[KitchenGameManager] Player " + senderClientId + " paused the game.");
        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        if (!playerPausedDictionary.ContainsKey(senderClientId)) {
            playerPausedDictionary[senderClientId] = false; // Default initialization
            Debug.LogWarning("[KitchenGameManager] Adding default entry for playerPausedDictionary for client: " + senderClientId);
        }

        playerPausedDictionary[senderClientId] = false;
        Debug.Log("[KitchenGameManager] Player " + senderClientId + " unpaused the game.");
        TestGamePausedState();
    }


    private void TestGamePausedState() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId]) {
                Debug.Log("[KitchenGameManager] Game paused because player " + clientId + " is paused.");
                isGamePaused.Value = true;
                return;
            }
        }
        Debug.Log("[KitchenGameManager] All players unpaused. Game resuming.");
        isGamePaused.Value = false;
    }
}
