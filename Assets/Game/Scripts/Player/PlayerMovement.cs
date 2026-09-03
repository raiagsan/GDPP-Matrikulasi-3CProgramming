using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    private float _rotationSmoothVelocity;

    [Header("Player Sprint")]
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _walkSprintTransition;

    [Header("Player Jump")]
    [SerializeField] private float _jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundDetector;
    [SerializeField] private float _detectorRadius;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    [Header("Player Ladder Movement")]
    [SerializeField] private Vector3 _upperStepOffset;
    [SerializeField] private float _stepCheckerDistance;
    [SerializeField] private float _stepForce;

    [Header("Player Climb")]
    [SerializeField] private Transform _climbDetector;
    [SerializeField] private float _climbCheckDistance;
    [SerializeField] private LayerMask _climbableLayer;
    [SerializeField] private Vector3 _climbOffset;
    [SerializeField] private float _climbSpeed;

    private float _speed;
    private Rigidbody _rigidbody;
    private PlayerStance _playerStance;


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerStance = PlayerStance.Stand;
        _speed = _walkSpeed;
    }

    void Update()
    {
        CheckIsGrounded();
        CheckStep();
    }

    void FixedUpdate()
    {
        Sprint();
        Move();
    }

    void OnEnable()
    {
        _inputManager.OnJumpTriggered += Jump;
        _inputManager.OnClimbTriggered += StartClimb;
        _inputManager.OnCancelClimborGlideTriggered += CancelClimb;
    }

    void OnDisable()
    {
        _inputManager.OnJumpTriggered -= Jump;
        _inputManager.OnClimbTriggered -= StartClimb;
        _inputManager.OnCancelClimborGlideTriggered -= CancelClimb;
    }

    private void Move()
    {
        Vector2 input = _inputManager.MoveInput;
        Vector3 movementDirection = Vector3.zero;
        bool isPlayerStanding = _playerStance == PlayerStance.Stand;
        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;

        if (isPlayerStanding){
            if (input.magnitude >= 0.1) {
                float rotationAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;

                float targetX = movementDirection.x * _speed;
                float targetZ = movementDirection.z * _speed;

                _rigidbody.linearVelocity = new Vector3(targetX, _rigidbody.linearVelocity.y, targetZ);
            }
            else
            {
                _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
            }
        }else if (isPlayerClimbing)
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
        }
    }

    private void CheckIsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _detectorRadius, _groundLayer);
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
        }
    }
}
