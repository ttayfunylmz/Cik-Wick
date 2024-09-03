using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
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
