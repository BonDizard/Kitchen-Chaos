/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Game Manger 
 */

using System;
using UnityEngine;
public enum State {
    WaitingToStart,
    CoolDownToStart,
    GamePlaying,
    GameOver,
}
public class KitchenGameManger : MonoBehaviour {
    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;

    public static KitchenGameManger Instance { get; private set; }
    float countDownToStartTimer = 3f;
    float gamePlayingTimer;
    float gamePlayingTimerMax = 10f;
    bool isGamePaused = false;
    public State state;
    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
    }
    private void Start() {
        GameInput.Instance.OnPauseButtonClicked += GameInput_OnPauseButtonCLicked;
        GameInput.Instance.OnInteractionPerformed += GameInput_OnInteractionPerformed;
    }
    private void GameInput_OnInteractionPerformed(object sender, EventArgs e) {
        if (state == State.WaitingToStart) {
            state = State.CoolDownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseButtonCLicked(object sender, EventArgs e) {
        TogglePauseGame();
    }
    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                break;
            case State.CoolDownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer < 0f) {
                    state = State.GamePlaying;
                }
                gamePlayingTimer = gamePlayingTimerMax;
                OnStateChanged?.Invoke(this, EventArgs.Empty);

                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f) {
                    state = State.GameOver;
                }
                OnStateChanged?.Invoke(this, EventArgs.Empty);

                break;
            case State.GameOver:
                //do nothing
                break;
            default: break;
        }
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }
    public bool IsInCountDownState() {
        return state == State.CoolDownToStart;
    }
    public float GetCountDownTime() {
        return countDownToStartTimer;
    }
    public bool IsGameOver() {
        return state == State.GameOver;
    }
    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
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
}
