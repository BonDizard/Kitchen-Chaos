/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Descriptions: Music Manager
 */
using UnityEngine;

public class MusicManger : MonoBehaviour {
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    public static MusicManger Instance { get; private set; }
    private float volume = 0.3f;
    private AudioSource audioSource;
    private void Awake() {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1f) {
            volume = 0f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume() {
        return volume;
    }
}
