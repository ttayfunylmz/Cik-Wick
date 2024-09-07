using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GoldWheatCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private WheatDesignSO _wheatDesignSO;

    [Header("Settings")]
    [SerializeField] private float _movementIncreaseSpeed;


    private RectTransform _playerBoosterTransform;
    private Image _playerBoosterImage;
    private GameObject _particlesPrefab;
    private float _particlesDestroyDuration;
    private float _resetBoostDuration;
    private Sprite _activeSprite;
    private Sprite _passiveSprite;
    private Sprite _activeWheatSprite;
    private Sprite _passiveWheatSprite;

    private PlayerController _playerController;
    private PlayerStateUI _playerStateUI;

    [Inject]
    private void ZenjectSetup(PlayerController playerController, PlayerStateUI playerStateUI)
    {
        _playerController = playerController;
        _playerStateUI = playerStateUI;
    }

    private void Awake() 
    {
        _playerBoosterTransform = _playerStateUI.GetBoosterSpeedTransform;
        _playerBoosterImage = _playerBoosterTransform.GetComponent<Image>();
        InitializeSO();
    }

    public void InitializeSO()
    {
        _particlesPrefab = _wheatDesignSO._particlesPrefab;
        _particlesDestroyDuration = _wheatDesignSO._particlesDestroyDuration;
        _resetBoostDuration = _wheatDesignSO._resetBoostDuration;
        _activeSprite = _wheatDesignSO._activeSprite;
        _passiveSprite = _wheatDesignSO._passiveSprite;
        _activeWheatSprite = _wheatDesignSO._activeWheatSprite;
        _passiveWheatSprite = _wheatDesignSO._passiveWheatSprite;
    }

    public void Collect()
    {
        _playerController.SetMovementSpeed(_movementIncreaseSpeed, _resetBoostDuration);
        _playerStateUI.PlayBoosterUIAnimations(_playerBoosterTransform, _playerBoosterImage, _playerStateUI.GetGoldBoosterWheatImage, 
            _activeWheatSprite, _passiveWheatSprite, _activeSprite, _passiveSprite, _resetBoostDuration);
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
