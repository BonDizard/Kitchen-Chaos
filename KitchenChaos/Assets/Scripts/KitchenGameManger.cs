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
    public static KitchenGameManger Instance { get; private set; }
    float waitingToStartTimer = 1f;
    float countDownToStartTimer = 3f;
    float playingTimer = 10f;
    public State state;
    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) {
                    state = State.CoolDownToStart;
                }
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;
            case State.CoolDownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer < 0f) {
                    state = State.GamePlaying;
                }
                OnStateChanged?.Invoke(this, EventArgs.Empty);

                break;
            case State.GamePlaying:
                playingTimer -= Time.deltaTime;
                if (playingTimer < 0f) {
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
}
