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

    public event Action OnJumpTriggered;
    public event Action OnClimbTriggered;
    public event Action OnCancelClimborGlideTriggered;
    public event Action OnChangePOVTriggered;

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
        Debug.Log("Sprinting");
    }

    void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _isSprinting = false;
        Debug.Log("Not Sprinting");
    }

    void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        OnJumpTriggered?.Invoke();
    }

    void OnCrouch(InputAction.CallbackContext context){Debug.Log("Crouch");}

    void OnChangePOV(InputAction.CallbackContext context)
    {
        Debug.Log("Change POV");
        OnChangePOVTriggered?.Invoke();
    }

    void OnGlide(InputAction.CallbackContext context){Debug.Log("Glide");}

    void OnClimb(InputAction.CallbackContext context)
    {
        Debug.Log("Climb");
        OnClimbTriggered?.Invoke();
    }

    void OnCancelClimborGlide(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel climb/glide");
        OnCancelClimborGlideTriggered?.Invoke();
    }

    void OnPunch(InputAction.CallbackContext context){Debug.Log("Punch");}

    void OnPause(InputAction.CallbackContext context){Debug.Log("Pause to main menu");}
}
