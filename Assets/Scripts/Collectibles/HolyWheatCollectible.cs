using UnityEngine;
using Zenject;

public class HolyWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private WheatDesignSO _wheatDesignSO;

    [Header("Settings")]
    [SerializeField] private float _forceIncrease;

    private GameObject _particlesPrefab;
    private float _particlesDestroyDuration;
    private float _resetBoostDuration;

    private PlayerController _playerController;

    [Inject]
    private void ZenjectSetup(PlayerController playerController)
    {
        _playerController = playerController;
    }

    private void Awake() 
    {
        InitializeSO();    
    }

    public void InitializeSO()
    {
        _particlesPrefab = _wheatDesignSO._particlesPrefab;
        _particlesDestroyDuration = _wheatDesignSO._particlesDestroyDuration;
        _resetBoostDuration = _wheatDesignSO._resetBoostDuration;
    }

    public void Collect()
    {
        _playerController.SetJumpForce(_forceIncrease, _resetBoostDuration);
        Destroy(gameObject);
    }

    public void PlayParticle(Transform playerTransform)
    {
        GameObject particleInstance = 
            Instantiate(_particlesPrefab, playerTransform.position, _particlesPrefab.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _particlesDestroyDuration);

    }
}
