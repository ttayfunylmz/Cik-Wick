using UnityEngine;
using Zenject;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _playerVisualTransform;
    [SerializeField] private Rigidbody _playerRigidbody;

    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;

    private GameManager _gameManager;

    [Inject]
    private void ZenjectSetup(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Start() 
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;    
    }

    private void Update() 
    {
        if(_gameManager.GetCurrentGameState() != GameState.Play 
            && _gameManager.GetCurrentGameState() != GameState.Resume) { return; }

        Vector3 viewDirection = 
            _playerTransform.position - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);

        _orientationTransform.forward = viewDirection.normalized;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = _orientationTransform.forward * verticalInput + _orientationTransform.right * horizontalInput;

        if(inputDirection != Vector3.zero)
        {
            _playerVisualTransform.forward = 
                Vector3.Slerp(_playerVisualTransform.forward, inputDirection.normalized, Time.deltaTime * _rotationSpeed);
        }
    }
}
