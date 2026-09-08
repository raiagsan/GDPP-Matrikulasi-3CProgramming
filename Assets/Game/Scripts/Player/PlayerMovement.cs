using UnityEditor.Animations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    private float _rotationSmoothVelocity;

    [Header("Sprint")]
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _walkSprintTransition;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundDetector;
    [SerializeField] private float _detectorRadius;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    [Header("Ladder Movement")]
    [SerializeField] private Vector3 _upperStepOffset;
    [SerializeField] private float _stepCheckerDistance;
    [SerializeField] private float _stepForce;

    [Header("Player Climb")]
    [SerializeField] private Transform _climbDetector;
    [SerializeField] private float _climbCheckDistance;
    [SerializeField] private LayerMask _climbableLayer;
    [SerializeField] private Vector3 _climbOffset;
    [SerializeField] private float _climbSpeed;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Crouch")]
    [SerializeField] private float _crouchSpeed;

    private float _speed;
    private Rigidbody _rigidbody;
    private PlayerStance _playerStance;
    private Animator _animator;


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerStance = PlayerStance.Stand;
        _speed = _walkSpeed;
        HideAndLockCursor();
        
    }

    void Update()
    {
        CheckStep();
    }

    void FixedUpdate()
    {
        Sprint();
        Move();
        CheckIsGrounded();
    }

    void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        _inputManager.OnJumpTriggered += Jump;
        _inputManager.OnClimbTriggered += StartClimb;
        _inputManager.OnCancelClimborGlideTriggered += CancelClimb;
        _inputManager.OnCrouchTriggered += Crouch;
        _cameraManager.OnChangePerspective += ChangePerspective;
    }

    void OnDisable()
    {
        _inputManager.OnJumpTriggered -= Jump;
        _inputManager.OnClimbTriggered -= StartClimb;
        _inputManager.OnCancelClimborGlideTriggered -= CancelClimb;
        _inputManager.OnCrouchTriggered -= Crouch;
        _cameraManager.OnChangePerspective -= ChangePerspective;
    }

    private void Move()
    {
        Vector2 input = _inputManager.MoveInput;
        Vector3 movementDirection = Vector3.zero;
        bool isPlayerStanding = _playerStance == PlayerStance.Stand;
        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;
        bool isPlayerCrouch = _playerStance == PlayerStance.Crouch;

        if (isPlayerStanding || isPlayerCrouch)
        {
            switch (_cameraManager.CameraState)
            {
                case CameraState.ThirdPersonCamera:
                    if (input.magnitude >= 0.1) {
                        float rotationAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                        movementDirection = (Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward).normalized;

                        _rigidbody.linearVelocity = new Vector3(movementDirection.x * _speed, _rigidbody.linearVelocity.y, movementDirection.z * _speed);
                    }
                    else
                    {
                        _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
                    }
                    break;
                case CameraState.FirstPersonCamera:
                    transform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
                    Vector3 verticalDirection = input.y * transform.forward;
                    Vector3 horizontalDirection = input.x * transform.right;
                    movementDirection = (verticalDirection + horizontalDirection).normalized;

                    _rigidbody.linearVelocity = new Vector3 (movementDirection.x * _speed, _rigidbody.linearVelocity.y, movementDirection.z * _speed);
                    break;
                default:
                    break;
            }
            Vector3 velocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            _animator.SetFloat("Velocity", velocity.magnitude * input.magnitude, 0.1f, Time.fixedDeltaTime);
            _animator.SetFloat("VelocityX", velocity.magnitude * input.x, 0.1f, Time.fixedDeltaTime);
            _animator.SetFloat("VelocityZ", velocity.magnitude * input.y, 0.1f, Time.fixedDeltaTime);
        }
        else if (isPlayerClimbing)
        {
            Vector3 horizontal = input.x * transform.right;
            Vector3 vertical = input.y * transform.up;
            movementDirection = (horizontal + vertical).normalized;
            _rigidbody.linearVelocity = (movementDirection * _climbSpeed);
        }
    }

    private void Sprint()
    {
        bool isSprint = _inputManager.IsSprinting;

        if (isSprint)
        {
            if (_speed < _sprintSpeed)
            {
                _speed = _speed + _walkSprintTransition * Time.fixedDeltaTime;
            }
        }
        else
        {
            if (_speed > _walkSpeed)
            {
                _speed = _speed - +_walkSprintTransition * Time.fixedDeltaTime;
            }
        }
    }

    private void Jump()
    {
        if (_isGrounded){
            Vector3 jumpDirection = Vector3.up;
            _rigidbody.AddForce((jumpDirection * _jumpForce), ForceMode.Impulse);
            _animator.SetTrigger("Jump");
        }
    }

    private void CheckIsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _detectorRadius, _groundLayer);
        _animator.SetBool("IsGrounded", _isGrounded);
    }

    private void CheckStep()
    {
        bool isHitLowerStep = Physics.Raycast(_groundDetector.position, transform.forward, _stepCheckerDistance);
        bool isHitUpperStep = Physics.Raycast(_groundDetector.position + _upperStepOffset, transform.forward, _stepCheckerDistance);

        if (isHitLowerStep && !isHitUpperStep)
        {
            _rigidbody.AddForce(0f, _stepForce, 0f);
        }
    }

    private void StartClimb()
    {
        bool isInFrontOfClimbingWall = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit hit, _climbCheckDistance, _climbableLayer);
        bool isNotClimbing = _playerStance != PlayerStance.Climb;

        if (isInFrontOfClimbingWall && _isGrounded && isNotClimbing)
        {
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            transform.position = hit.point - offset;
            _playerStance = PlayerStance.Climb;
            _rigidbody.useGravity = false;
            _speed = _climbSpeed;
            _cameraManager.SetFPSClampedCamera(true, transform.rotation.eulerAngles);
            _cameraManager.SetTPSFieldOfView(70f);
        }
    }

    private void CancelClimb()
    {
        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;
        
        if (isPlayerClimbing)
        {
            _playerStance = PlayerStance.Stand;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward * 1f;
            _speed = _walkSpeed;
            _cameraManager.SetFPSClampedCamera(false, transform.rotation.eulerAngles);
            _cameraManager.SetTPSFieldOfView(40f);
        }
    }

    private void ChangePerspective()
    {
        _animator.SetTrigger("ChangePerspective");
    }

    public void Crouch()
    {
        if (_playerStance == PlayerStance.Stand)
        {
            _playerStance = PlayerStance.Crouch;
            _animator.SetBool("IsCrouch", true);
            _speed = _crouchSpeed;
        }
        else if (_playerStance == PlayerStance.Crouch)
        {
            _playerStance = PlayerStance.Stand;
            _animator.SetBool("IsCrouch", false);
            _speed = _walkSpeed;
        }
    }
}
