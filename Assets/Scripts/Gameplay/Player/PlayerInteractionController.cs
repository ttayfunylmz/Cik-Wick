using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        collectible?.Collect();
        collectible?.PlayParticle(transform);
    }
}
