using UnityEngine;
using UnityEngine.AI;

public class CatController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private float _patrolRadius = 10f;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private int _maxDestinationAttempts = 10;

    private CatStateController _catStateController;
    
    private NavMeshAgent _catAgent;
    private Vector3 _initialPosition;
    private float _timer;
    private bool _isWaiting;

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
            Vector3 randomDirection = Random.insideUnitSphere * _patrolRadius;
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
