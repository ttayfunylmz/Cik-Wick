using UnityEngine;

public interface IBoostable
{
    void Boost(PlayerController playerController);
    void PlayBoostAnimation();   
    void PlayBoostParticle(Transform playerTransform);
}
