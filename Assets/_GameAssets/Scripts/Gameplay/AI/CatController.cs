using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Zenject.SpaceFighter;

public class CatController : MonoBehaviour
{
    public event Action<Transform> OnCatCatched;

    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private CinemachineCamera _catCinemachineCamera;

    [Header("Settings")]
    [SerializeField] private float _defaultSpeed = 5f;
    [SerializeField] private float _chaseSpeed = 7f;

    [Header("Navigation Settings")]
    [SerializeField] private float _patrolRadius = 10f;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private int _maxDestinationAttempts = 10;
    [SerializeField] private float _chaseDistanceThreshold = 1.5f;
    [SerializeField] private float _chaseDistance = 2f;

    private NavMeshAgent _catAgent;
    private Vector3 _initialPosition;
    private float _timer;
    private bool _isWaiting;
    [SerializeField] private bool _isChasing = true;

    private CatStateController _catStateController;
    private PlayerController _playerController;
    private PlayerHealthUI _playerHealthUI;
    private CameraShake _cameraShake;

    [Inject]
    private void ZenjectSetup(PlayerController playerController, PlayerHealthUI playerHealthUI,
        CameraShake cameraShake)
    {
        _playerController = playerController;
        _playerHealthUI = playerHealthUI;
        _cameraShake = cameraShake;
    }

    private void Awake() 
    {
        _catStateController = GetComponent<CatStateController>();
    }

    private void Start()
    {
        _catAgent = GetComponent<NavMeshAgent>();
        if (_catAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing from this GameObject. Please add one.");
            enabled = false;
            return;
        }

        _initialPosition = transform.position;

        SetRandomDestination();
    }

    private void Update()
    {
        if(!_playerController.CanCatChase())
        {
            SetPatrolMovement();
        }
        else
        {
            SetChaseMovement();
        }
    }

    private void SetChaseMovement()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        Vector3 offsetPosition = _playerTransform.position - directionToPlayer * _chaseDistanceThreshold;
        _catAgent.SetDestination(offsetPosition);
        _catAgent.speed = _chaseSpeed;
        _catStateController.ChangeState(CatState.Chasing);

        if (Vector3.Distance(transform.position, _playerTransform.position) <= _chaseDistance && _isChasing)
        {
            _catCinemachineCamera.Priority = 2;
            _catStateController.ChangeState(CatState.Catched);
            _cameraShake.ShakeCamera(1.5f, 2f, 0.5f);
            OnCatCatched?.Invoke(_playerTransform);
            _playerHealthUI.AnimateDamageForAll();
            _isChasing = false;
        }
    }

    private void SetPatrolMovement()
    {
        _catAgent.speed = _defaultSpeed;

        if (!_catAgent.pathPending && _catAgent.remainingDistance <= _catAgent.stoppingDistance)
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                _timer = _waitTime;
                _catStateController.ChangeState(CatState.Idle);
            }
        }

        if (_isWaiting)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _isWaiting = false;
                SetRandomDestination();
                _catStateController.ChangeState(CatState.Running);
            }
        }
    }

    private void SetRandomDestination()
    {
        int attempts = 0;
        bool destinationSet = false;

        while (attempts < _maxDestinationAttempts && !destinationSet)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _patrolRadius;
            randomDirection += _initialPosition;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
            {
                Vector3 finalPosition = hit.position;

                if (!IsPositionBlocked(finalPosition))
                {
                    _catAgent.SetDestination(finalPosition);
                    destinationSet = true;
                }
                else
                {
                    attempts++;
                }
            }
            else
            {
                attempts++;
            }
        }

        if (!destinationSet)
        {
            Debug.LogWarning("Failed to find a valid, unblocked destination after " + _maxDestinationAttempts + " attempts.");
            _isWaiting = true;
            _timer = _waitTime * 2;
        }
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        if (NavMesh.Raycast(transform.position, position, out NavMeshHit hit, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = (_initialPosition != Vector3.zero) ? _initialPosition : transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, _patrolRadius);
    }
}
