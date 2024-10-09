using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private List<Transform> _eggPlaces;
    [SerializeField] private Transform _playerVisualTransform;

    private PlayerController _playerController;
    private Rigidbody _playerRigidbody;

    private void Awake() 
    {
        _playerController = GetComponent<PlayerController>();
        _playerRigidbody = GetComponent<Rigidbody>();
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
        }
        else if(other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.GiveDamage(_playerRigidbody, _playerVisualTransform);
        }
    }
}
