using System;
using DG.Tweening;
using MaskTransitions;
using TextAnimation;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Zenject;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _settingsPopupObject;
    [SerializeField] private GameObject _blackBackgroundObject;
    [SerializeField] private TextAnimatorManager _textAnimatorManager;
    [SerializeField] private PlayableDirector _playableDirector;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Sprites")]
    [SerializeField] private Sprite _musicActiveSprite;
    [SerializeField] private Sprite _musicPassiveSprite;
    [SerializeField] private Sprite _soundActiveSprite;
    [SerializeField] private Sprite _soundPassiveSprite;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration;

    private Image _blackBackgroundImage;

    private bool _isMusicActive = true;
    private bool _isSoundActive = true;

    private GameManager _gameManager;
    private AudioManager _audioManager;
    private BackgroundMusic _backgroundMusic;

    [Inject]
    private void ZenjectSetup(GameManager gameManager, AudioManager audioManager, BackgroundMusic backgroundMusic)
    {
        _gameManager = gameManager;
        _audioManager = audioManager;
        _backgroundMusic = backgroundMusic;
    }

    private void Awake() 
    {
        _blackBackgroundImage = _blackBackgroundObject.GetComponent<Image>();
        _settingsPopupObject.transform.localScale = Vector3.zero;

        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _musicButton.onClick.AddListener(OnMusicButtonClicked);
        _soundButton.onClick.AddListener(OnSoundButtonClicked);
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void Start() 
    {
        _playableDirector.stopped += OnTimelineFinished;    
    }

    private void OnTimelineFinished(PlayableDirector playableDirector)
    {
        _settingsButton.interactable = true;
        _settingsButton.gameObject.GetComponent<ButtonHoverController>().enabled = true;
    }

    private void OnSettingsButtonClicked()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        _blackBackgroundObject.SetActive(true);
        _settingsPopupObject.SetActive(true);
        _blackBackgroundImage.DOFade(0.8f, _scaleDuration).SetEase(Ease.Linear);
        _settingsPopupObject.transform.DOScale(1.5f, _scaleDuration).SetEase(Ease.OutBack);
        _gameManager.ChangeGameState(GameState.Pause);
    }

    private void OnResumeButtonClicked()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        _blackBackgroundImage.DOFade(0f, _scaleDuration).SetEase(Ease.Linear);
        _settingsPopupObject.transform.DOScale(0f, _scaleDuration).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            _settingsPopupObject.SetActive(false);
            _gameManager.ChangeGameState(GameState.Resume);
            _blackBackgroundObject.SetActive(false);
        });
    }

    private void OnMusicButtonClicked()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        _isMusicActive = !_isMusicActive;
        _musicButton.image.sprite = _isMusicActive ? _musicActiveSprite : _musicPassiveSprite;
        _backgroundMusic.SetMusicMute(!_isMusicActive);
    }

    private void OnSoundButtonClicked()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        _isSoundActive = !_isSoundActive;
        _soundButton.image.sprite = _isSoundActive ? _soundActiveSprite : _soundPassiveSprite;
        _audioManager.SetSoundEffectsMute(!_isSoundActive);
    }

    private void OnMainMenuButtonClicked()
    {
        Destroy(_textAnimatorManager.gameObject);
        _audioManager.Play(SoundType.TransitionSound);
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
    }

    private void OnDestroy() 
    {
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        _musicButton.onClick.RemoveListener(OnMusicButtonClicked);
        _soundButton.onClick.RemoveListener(OnSoundButtonClicked);
        _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);    
    }

}
