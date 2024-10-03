/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Code for an object to follow another object.
 */

using UnityEngine;

public class FollowTransform : MonoBehaviour {
    private Transform transformToFollow;
    public void SetTransformToFollow(Transform transform) {
        transformToFollow = transform;
    }
    private void LateUpdate() {
        transform.position = transformToFollow.position;
        transform.rotation = transformToFollow.rotation;
    }
}
