using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _settingsPopupObject;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _closeButton;
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

    private bool _isMusicActive = true;
    private bool _isSoundActive = true;

    private void Awake() 
    {
        _settingsPopupObject.transform.localScale = Vector3.zero;

        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _musicButton.onClick.AddListener(OnMusicButtonClicked);
        _soundButton.onClick.AddListener(OnSoundButtonClicked);
        _resumeButton.onClick.AddListener(OnCloseButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnSettingsButtonClicked()
    {
        _settingsPopupObject.SetActive(true);
        _settingsPopupObject.transform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        //TODO: Pause the Game.
    }

    private void OnCloseButtonClicked()
    {
        _settingsPopupObject.transform.DOScale(0f, _scaleDuration).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            _settingsPopupObject.SetActive(false);
            //TODO: Resume the Game.
        });
    }

    private void OnMusicButtonClicked()
    {
        _isMusicActive = !_isMusicActive;
        _musicButton.image.sprite = _isMusicActive ? _musicActiveSprite : _musicPassiveSprite;
    }

    private void OnSoundButtonClicked()
    {
        _isSoundActive = !_isSoundActive;
        _soundButton.image.sprite = _isSoundActive ? _soundActiveSprite : _soundPassiveSprite;
    }

    private void OnMainMenuButtonClicked()
    {
        //TODO: Go to Main Menu.
    }

    private void OnDestroy() 
    {
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        _musicButton.onClick.RemoveListener(OnMusicButtonClicked);
        _soundButton.onClick.RemoveListener(OnSoundButtonClicked);
        _resumeButton.onClick.RemoveListener(OnCloseButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);    
    }

}
