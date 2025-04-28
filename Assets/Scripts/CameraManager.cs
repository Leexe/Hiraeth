using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager> {
    [SerializeField] private CinemachineCamera _firstPersonCamera;

    public void ChangeCameraToFirstPerson() {
        _firstPersonCamera.Priority = 1;
    }
}
