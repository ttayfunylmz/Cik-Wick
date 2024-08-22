using UnityEngine;

public class RottenWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private GameObject _rottenParticles;

    [Header("Settings")]
    [SerializeField] private float _destroyDuration;

    public void Collect()
    {
        Debug.Log("Rotten Wheat collected!");
        Destroy(gameObject);
    }

    public void PlayParticle(Transform playerTransform)
    {
        GameObject particleInstance = 
            Instantiate(_rottenParticles, playerTransform.position, _rottenParticles.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _destroyDuration);
    }
}
