
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    public enum Mode {
        LookAt,
        LookInverted,
        CameraForward,
        CameraInverted,
    }
    [SerializeField] public Mode mode;
    //runs after the update
    private void LateUpdate() {
        switch (mode) {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookInverted:
                Vector3 directionOfTheCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(directionOfTheCamera + transform.position);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
        transform.LookAt(Camera.main.transform);
    }
}