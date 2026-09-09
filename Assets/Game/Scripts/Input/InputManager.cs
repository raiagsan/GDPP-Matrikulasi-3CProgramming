using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private Vector2 _moveInput;
    private bool _isSprinting;

    public Vector2 MoveInput => _moveInput;
    public bool IsSprinting => _isSprinting;

    public Action OnClimbTriggered;
    public Action OnCancelClimbTriggered;
    public Action OnChangePOVTriggered;
    public Action OnCrouchTriggered;
    public Action OnJumpTriggered;
    public Action OnGlideTriggered;
    public Action OnCancelGlideTriggered;
    public Action OnPunchTriggered;
    public Action OnPauseTriggered;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        _playerInputActions.Player.Move.performed += OnMovePerformed;
        _playerInputActions.Player.Move.canceled += OnMovePerformed;
        _playerInputActions.Player.Sprint.performed += OnSprintPerformed;
        _playerInputActions.Player.Sprint.canceled += OnSprintCanceled;
        _playerInputActions.Player.Jump.performed += OnJump;
        _playerInputActions.Player.Crouch.performed += OnCrouch;
        _playerInputActions.Player.ChangePOV.performed += OnChangePOV;
        _playerInputActions.Player.Glide.performed += OnGlide;
        _playerInputActions.Player.Climb.performed += OnClimb;
        _playerInputActions.Player.CancelClimborGlide.performed += OnCancelClimborGlide;
        _playerInputActions.Player.Punch.performed += OnPunch;
        _playerInputActions.Player.Pause.performed += OnPause;
        _playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        _playerInputActions.Player.Move.performed -= OnMovePerformed;
        _playerInputActions.Player.Move.canceled -= OnMovePerformed;
        _playerInputActions.Player.Sprint.performed -= OnSprintPerformed;
        _playerInputActions.Player.Sprint.canceled -= OnSprintCanceled;
        _playerInputActions.Player.Jump.performed -= OnJump;
        _playerInputActions.Player.Crouch.performed -= OnCrouch;
        _playerInputActions.Player.ChangePOV.performed -= OnChangePOV;
        _playerInputActions.Player.Glide.performed -= OnGlide;
        _playerInputActions.Player.Climb.performed -= OnClimb;
        _playerInputActions.Player.CancelClimborGlide.performed -= OnCancelClimborGlide;
        _playerInputActions.Player.Punch.performed -= OnPunch;
        _playerInputActions.Player.Pause.performed -= OnPause;
        _playerInputActions.Player.Disable();
    }

    void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        Debug.Log($"Move input: {_moveInput}");
    }

    void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _isSprinting = true;
    }

    void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _isSprinting = false;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        OnJumpTriggered?.Invoke();
    }

    void OnCrouch(InputAction.CallbackContext context)
    {
        OnCrouchTriggered?.Invoke();
    }

    void OnChangePOV(InputAction.CallbackContext context)
    {
        OnChangePOVTriggered?.Invoke();
    }

    void OnGlide(InputAction.CallbackContext context)
    {
        OnGlideTriggered?.Invoke();
    }

    void OnClimb(InputAction.CallbackContext context)
    {
        OnClimbTriggered?.Invoke();
    }

    void OnCancelClimborGlide(InputAction.CallbackContext context)
    {
        OnCancelClimbTriggered?.Invoke();
        OnCancelGlideTriggered?.Invoke();
    }
    void OnPunch(InputAction.CallbackContext context)
    {
        OnPunchTriggered?.Invoke();
    }

    void OnPause(InputAction.CallbackContext context)
    {
        OnPauseTriggered?.Invoke();
    }
}
