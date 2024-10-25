using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerVisualTransform;

    [Header("Settings")]
    [SerializeField] private float _interactRange = 4f;

    private PlayerController _playerController;
    private Rigidbody _playerRigidbody;

    private GameManager _gameManager;
    private CameraShake _cameraShake;

    [Inject]
    private void ZenjectSetup(GameManager gameManager, CameraShake cameraShake)
    {
        _gameManager = gameManager;
        _cameraShake = cameraShake;
    }

    private void Awake() 
    {
        _playerController = GetComponent<PlayerController>();
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        if(_gameManager.GetCurrentGameState() != GameState.Play 
            && _gameManager.GetCurrentGameState() != GameState.Resume) { return; }

        if(Input.GetKeyDown(KeyCode.F))
        {
            IInteractable interactable = GetInteractableObject();
            interactable?.Interact(transform);
        }    
    }

    private void OnTriggerEnter(Collider other) 
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        collectible?.Collect();
        collectible?.PlayParticle(transform);
        collectible?.PlayHitParticle(transform);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.TryGetComponent<IBoostable>(out var boostable))
        {
            boostable.Boost(_playerController);
            boostable.PlayBoostParticle(transform);
        }
        else if(other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.GiveDamage(_playerRigidbody, _playerVisualTransform);
            damageable.PlayHitParticle(transform);
        }
    }

    private void OnParticleCollision(GameObject other) 
    {
        if(other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.GiveDamage(_playerRigidbody, _playerVisualTransform);
            damageable.PlayHitParticle(transform);

            _cameraShake.ShakeCamera(1f, 0.5f);
        }    
    }

    public IInteractable GetInteractableObject()
    {
        List<IInteractable> interactableList = new List<IInteractable>();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, _interactRange);

        foreach (Collider collider in colliderArray)
        {
            if(collider.TryGetComponent(out IInteractable interactable))
            {
                interactableList.Add(interactable);
            }
        }

        IInteractable closestInteractable = null;
        foreach(IInteractable interactable in interactableList)
        {
            if(closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if(Vector3.Distance(transform.position, interactable.GetTransform().position) <
                    Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
                {
                    closestInteractable = interactable;
                }
            }
        }

        return closestInteractable;
    }
}
