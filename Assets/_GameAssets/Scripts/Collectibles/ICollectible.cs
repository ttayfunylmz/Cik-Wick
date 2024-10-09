using UnityEngine;

public interface ICollectible 
{
    void Collect();
    void PlayParticle(Transform playerTransform);
    void PlayHitParticle(Transform playerTransform);
}
