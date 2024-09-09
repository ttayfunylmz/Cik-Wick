using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private List<Transform> _eggPlaces;

    private PlayerController _playerController;

    private void Awake() 
    {
        _playerController = GetComponent<PlayerController>();    
    }

    private void OnTriggerEnter(Collider other) 
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        collectible?.Collect();
        collectible?.PlayParticle(transform);
    }

    private void OnCollisionEnter(Collision other) 
    {
        IBoostable boostable = other.gameObject.GetComponent<IBoostable>();
        boostable?.Boost(_playerController);
        boostable?.PlayBoostAnimation();
    }
}
