using UnityEngine;

public class GoldWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private GameObject _goldParticles;

    [Header("Settings")]
    [SerializeField] private float _destroyDuration;

    public void Collect()
    {
        Debug.Log("Gold Wheat collected!");
        Destroy(gameObject);
    }

    public void PlayParticle(Transform playerTransform)
    {
        GameObject particleInstance = 
            Instantiate(_goldParticles, playerTransform.position, _goldParticles.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _destroyDuration);
    }
}
