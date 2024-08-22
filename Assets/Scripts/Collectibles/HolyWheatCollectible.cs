using UnityEngine;

public class HolyWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private GameObject _holyParticles;

    [Header("Settings")]
    [SerializeField] private float _destroyDuration;

    public void Collect()
    {
        Debug.Log("Holy Wheat collected!");
        Destroy(gameObject);
    }

    public void PlayParticle(Transform playerTransform)
    {
        GameObject particleInstance = 
            Instantiate(_holyParticles, playerTransform.position, _holyParticles.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _destroyDuration);

    }
}
