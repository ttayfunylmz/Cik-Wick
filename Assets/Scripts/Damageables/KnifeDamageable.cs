using UnityEngine;
using Zenject;

public class KnifeDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private float _force = 10f;

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
}
