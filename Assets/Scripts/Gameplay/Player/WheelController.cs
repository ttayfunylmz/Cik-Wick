using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _leftWheelTransform;
    [SerializeField] private Transform _rightWheelTransform;

    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;

    private PlayerController _playerController;

    private float _horizontalInput, _verticalInput;

    private void Awake() 
    {
        _playerController = GetComponent<PlayerController>();    
    }

    private void Update()
    {
        SetWheelRotations();
    }


    private void SetWheelRotations()
    {
        if(_playerController.GetMovementDirection() != Vector3.zero)
        {
                _leftWheelTransform.Rotate(_rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);
                _rightWheelTransform.Rotate(-_rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);
        }
    }

}
