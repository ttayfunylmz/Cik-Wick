using UnityEngine;
using Zenject;

public class FireDamageable : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject _hitParticlesPrefab;

    [Header("Settings")]
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _hitParticlesDestroyDuration = 2f;

    private HealthManager _healthManager;

    [Inject]
    private void ZenjectSetup(HealthManager healthManager)
    {
        _healthManager = healthManager;
    }

    public void GiveDamage(Rigidbody playerRigidbody, Transform playerVisualTransform)
    {
        _healthManager.Damage(1);
        playerRigidbody.AddForce(-playerVisualTransform.forward * _force, ForceMode.Impulse);
        Destroy(gameObject);
    }

    public void PlayHitParticle(Transform playerTransform)
    {
        Vector3 offset = new Vector3(0f, 0.7f, 0f);

        GameObject particleInstance =
            Instantiate(_hitParticlesPrefab, playerTransform.position + offset, _hitParticlesPrefab.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _hitParticlesDestroyDuration);
    }
}
