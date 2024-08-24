using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerJumped;

    [Header("References")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    [SerializeField] private bool _canJump;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _playerHeight;
    [SerializeField] private float _groundDrag;

    private float _horizontalInput, _verticalInput;
    private float _startingMovementSpeed;
    private Vector3 _movementDirection;
    private Rigidbody _playerRigidbody;
    private bool _isGrounded;

    private void Awake() 
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
        _startingMovementSpeed = _movementSpeed;
    }

    private void Update() 
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);

        SetInputs();
        SetPlayerSpeed();

        if(_isGrounded)
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
        else
        {
            _playerRigidbody.linearDamping = 0f;
        }
    }

    private void FixedUpdate() 
    {
        SetPlayerMovement();    
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(_jumpKey) && _canJump && _isGrounded)
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;

        if(_isGrounded)
        {
            _playerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
        }
        else
        {
            _playerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed * _airMultiplier, ForceMode.Force);
        }
    }

    private void SetPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);

        if(flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void SetPlayerJumping()
    {
        OnPlayerJumped?.Invoke();
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJumping()
    {
        _canJump = true;
    }

    public Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }

    public void ResetMovementSpeed()
    {
        _movementSpeed = _startingMovementSpeed;
    }

    public void SetMovementSpeed(float speed, float duration)
    {
        _movementSpeed += speed;
        Invoke(nameof(ResetMovementSpeed), duration);
    }

}
