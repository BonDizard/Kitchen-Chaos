/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Scriptable object for Audio clips to be in one place
 */
using UnityEngine;
[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject {
    public AudioClip[] chop;
    public AudioClip[] deliveryFail;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footstep;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip stoveSizzle;
    public AudioClip[] trash;
    public AudioClip[] warning;
}
