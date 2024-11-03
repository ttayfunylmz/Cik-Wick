using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HolyWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private WheatDesignSO _wheatDesignSO;

    [Header("Settings")]
    [SerializeField] private float _forceIncrease;


    private RectTransform _playerBoosterTransform;
    private Image _playerBoosterImage;
    private GameObject _particlesPrefab;
    private GameObject _hitParticlesPrefab;
    private float _particlesDestroyDuration;
    private float _resetBoostDuration;
    private float _hitParticlesDestroyDuration;
    private Sprite _activeSprite;
    private Sprite _passiveSprite;
    private Sprite _activeWheatSprite;
    private Sprite _passiveWheatSprite;

    private PlayerController _playerController;
    private PlayerStateUI _playerStateUI;
    private CameraShake _cameraShake;
    private AudioManager _audioManager;

    [Inject]
    private void ZenjectSetup(PlayerController playerController, PlayerStateUI playerStateUI,
        CameraShake cameraShake, AudioManager audioManager)
    {
        _playerController = playerController;
        _playerStateUI = playerStateUI;
        _cameraShake = cameraShake;
        _audioManager = audioManager;
    }

    private void Awake() 
    {
        _playerBoosterTransform = _playerStateUI.GetBoosterJumpTransform;
        _playerBoosterImage = _playerBoosterTransform.GetComponent<Image>();
        InitializeSO();    
    }

    public void InitializeSO()
    {
        _particlesPrefab = _wheatDesignSO.ParticlesPrefab;
        _hitParticlesPrefab = _wheatDesignSO.HitParticlesPrefab;
        _particlesDestroyDuration = _wheatDesignSO.ParticlesDestroyDuration;
        _resetBoostDuration = _wheatDesignSO.ResetBoostDuration;
        _hitParticlesDestroyDuration = _wheatDesignSO.HitParticlesDestroyDuration;
        _activeSprite = _wheatDesignSO.ActiveSprite;
        _passiveSprite = _wheatDesignSO.PassiveSprite;
        _activeWheatSprite = _wheatDesignSO.ActiveWheatSprite;
        _passiveWheatSprite = _wheatDesignSO.PassiveWheatSprite;
    }

    public void Collect()
    {
        _playerController.SetJumpForce(_forceIncrease, _resetBoostDuration);
        _playerStateUI.PlayBoosterUIAnimations(_playerBoosterTransform, _playerBoosterImage,_playerStateUI.GetHolyBoosterWheatImage, 
            _activeWheatSprite, _passiveWheatSprite, _activeSprite, _passiveSprite, _resetBoostDuration);

        _cameraShake.ShakeCamera(0.5f, 0.5f);

        _audioManager.Play(SoundType.PickupGoodSound);

        Destroy(gameObject);
    }

    public void PlayParticle(Transform playerTransform)
    {
        GameObject particleInstance = 
            Instantiate(_particlesPrefab, playerTransform.position, _particlesPrefab.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _particlesDestroyDuration);
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
