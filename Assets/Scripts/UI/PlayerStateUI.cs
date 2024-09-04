using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerStateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _playerWalkingTransform;
    [SerializeField] private RectTransform _playerSlidingTransform;

    [Header("Sprites")]
    [SerializeField] private Sprite _playerWalkingActiveSprite;
    [SerializeField] private Sprite _playerWalkingPassiveSprite;
    [SerializeField] private Sprite _playerSlidingActiveSprite;
    [SerializeField] private Sprite _playerSlidingPassiveSprite;

    [Header("Settings")]
    [SerializeField] private float _moveDuration;
    [SerializeField] private Ease _moveEase;

    private Image _playerWalkingImage;
    private Image _playerSlidingImage;

    private StateController _stateController;
    private PlayerController _playerController;

    [Inject]
    private void ZenjectSetup(StateController stateController, PlayerController playerController)
    {
        _stateController = stateController;
        _playerController = playerController;
    }

    private void Awake() 
    {
        _playerWalkingImage = _playerWalkingTransform.GetComponent<Image>();
        _playerSlidingImage = _playerSlidingTransform.GetComponent<Image>();    
    }

    private void Start()
    {
        _playerController.OnPlayerStateChanged += OnPlayerStateChanged;
        
        SetStateUserInterfaces(_playerWalkingActiveSprite, _playerSlidingPassiveSprite, _playerWalkingTransform, _playerSlidingTransform);
    }

    private void OnPlayerStateChanged(PlayerState playerState)
    {
        switch(playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                SetStateUserInterfaces(_playerWalkingActiveSprite, _playerSlidingPassiveSprite, _playerWalkingTransform, _playerSlidingTransform);
                break;
            
            case PlayerState.Slide:
            case PlayerState.SlideIdle:
                SetStateUserInterfaces(_playerWalkingPassiveSprite, _playerSlidingActiveSprite, _playerSlidingTransform, _playerWalkingTransform);
                break;
        }
    }

    private void SetStateUserInterfaces(Sprite playerWalkingSprite, Sprite playerSlidingSprite,
        RectTransform activeTransform, RectTransform passiveTransform)
    {
        _playerWalkingImage.sprite = playerWalkingSprite;
        _playerSlidingImage.sprite = playerSlidingSprite;

        activeTransform.DOAnchorPosX(-25f, _moveDuration).SetEase(_moveEase);
        passiveTransform.DOAnchorPosX(-90f, _moveDuration).SetEase(_moveEase);
    }
}
