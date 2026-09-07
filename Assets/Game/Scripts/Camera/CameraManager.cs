using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] public CameraState CameraState;
    [SerializeField] private CinemachinePanTilt _fpsCamera;
    [SerializeField] private CinemachineCamera _tpsCamera;
    [SerializeField] private InputManager _inputManager;

    public Action OnChangePerspective;

    void OnEnable()
    {
        _inputManager.OnChangePOVTriggered += SwitchCamera;
    }

    void OnDisable()
    {
        _inputManager.OnChangePOVTriggered -= SwitchCamera;
    }

    public void SetTPSFieldOfView(float fieldOfView)
    {
        _tpsCamera.Lens.FieldOfView = fieldOfView;    
    }

    public void SetFPSClampedCamera(bool isClamped, Vector3 playerRotation)
    {
        CinemachinePanTilt pov = _fpsCamera.GetComponent<CinemachinePanTilt>();
        if (isClamped)
        {
            pov.PanAxis.Wrap = false;
            pov.PanAxis.Range = new Vector2(
                playerRotation.y - 45f, 
                playerRotation.y + 45f
                );
        }
        else
        {
            pov.PanAxis.Range = new Vector2(-180f, 180f);
            pov.PanAxis.Wrap = true;
        }
    }

    public void SwitchCamera()
    {
        OnChangePerspective();
        if (CameraState == CameraState.FirstPersonCamera)
        {
            CameraState = CameraState.ThirdPersonCamera;
            _fpsCamera.gameObject.SetActive(false);
            _tpsCamera.gameObject.SetActive(true);

        }
        else
        {
            CameraState = CameraState.FirstPersonCamera;
            _tpsCamera.gameObject.SetActive(false);
            _fpsCamera.gameObject.SetActive(true);
        }
    }
}
