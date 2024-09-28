/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Delivery Logic handled here.
 */
using UnityEngine;

public class SoundManager : MonoBehaviour {
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    private void Awake() {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 0.3f);
    }
    private float volume;
    private void Start() {
        DeliverManager.Instance.OnRecipeSucess += DeliverManager_OnRecipeSucess;
        DeliverManager.Instance.OnRecipeFailed += DeliverManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        //Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }
    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
        PlaySound(audioClipRefsSO.trash, (sender as TrashCounter).transform.position);
    }
    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
        PlaySound(audioClipRefsSO.objectDrop, (sender as BaseCounter).transform.position);
    }
    private void Player_OnPickedSomething(object sender, System.EventArgs e) {
        //  PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
    }
    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }
    private void DeliverManager_OnRecipeSucess(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
    }
    private void DeliverManager_OnRecipeFailed(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
    }
    public void PlayWalkingSound(Vector3 position, float volume) {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }
    public void PlayCountDownSound() {
        PlaySound(audioClipRefsSO.warning, Vector2.zero);
    }
    public void PlayBuringWarningSound(Vector2 position) {
        PlaySound(audioClipRefsSO.warning, position);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultipler) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultipler);
    }
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1f) {
            volume = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume() {
        return volume;
    }
}
