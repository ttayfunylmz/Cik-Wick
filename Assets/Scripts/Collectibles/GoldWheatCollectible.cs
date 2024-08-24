using UnityEngine;
using Zenject;

public class GoldWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private WheatDesignSO _wheatDesignSO;

    private GameObject _particlesPrefab;
    private float _particlesDestroyDuration;

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
    }

    public void Collect()
    {
        _playerController.SetMovementSpeed(40f);
        Debug.Log("Gold Wheat collected!");
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
