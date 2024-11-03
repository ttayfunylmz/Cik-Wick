using System;
using MaskTransitions;
using TextAnimation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WinPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _oneMoreButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextAnimatorManager _textAnimatorManager;

    private TimerUI _timerUI;
    private AudioManager _audioManager;
    private BackgroundMusic _backgroundMusic;

    [Inject]
    private void ZenjectSetup(TimerUI timerUI, AudioManager audioManager, BackgroundMusic backgroundMusic)
    {
        _timerUI = timerUI;
        _audioManager = audioManager;
        _backgroundMusic = backgroundMusic;
    }

    private void OnEnable() 
    {
        SetTimerText();
        _audioManager.Play(SoundType.WinSound);
        _backgroundMusic.PlayBackgroundMusic(false);

        _oneMoreButton.onClick.AddListener(OnOneMoreButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnMainMenuButtonClicked()
    {
        Destroy(_textAnimatorManager.gameObject);
        _audioManager.Play(SoundType.TransitionSound);
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
    }

    private void OnOneMoreButtonClicked()
    {
        Destroy(_textAnimatorManager.gameObject);
        _audioManager.Play(SoundType.ButtonClickSound);
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE);
    }

    private void SetTimerText()
    {
        _timerText.text = _timerUI.GetFinalTime();
    }

    private void OnDisable() 
    {
        _oneMoreButton.onClick.RemoveListener(OnOneMoreButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);    
    }   
}
