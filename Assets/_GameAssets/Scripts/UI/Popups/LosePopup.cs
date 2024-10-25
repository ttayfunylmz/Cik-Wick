using System;
using MaskTransitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LosePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _tryAgainButton;
    [SerializeField] private Button _mainMenuButton;
    
    private TimerUI _timerUI;
    private AudioManager _audioManager;

    [Inject]
    private void ZenjectSetup(TimerUI timerUI, AudioManager audioManager)
    {
        _timerUI = timerUI;
        _audioManager = audioManager;
    }

    private void OnEnable() 
    {
        SetTimerText();

        _tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);    
    }

    private void OnMainMenuButtonClicked()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
    }

    private void OnTryAgainButtonClicked()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE);
    }
    
    private void SetTimerText()
    {
        _timerText.text = _timerUI.GetFinalTime();
    }

    private void OnDisable() 
    {
        _tryAgainButton.onClick.RemoveListener(OnTryAgainButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);    
    }

}
