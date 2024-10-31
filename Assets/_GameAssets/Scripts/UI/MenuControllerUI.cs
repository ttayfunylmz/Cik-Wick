using System;
using System.Collections;
using DG.Tweening;
using MaskTransitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MenuControllerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private RectTransform _headerImageTransform;

    [Header("Content References")]
    [SerializeField] private RectTransform _howToPlayContentTransform;
    [SerializeField] private RectTransform _creditsContentTransform;
    [SerializeField] private RectTransform _quitContentTransform;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private RectTransform _currentContentTransform = null;

    private AudioManager _audioManager;

    [Inject]
    private void ZenjectSetup(AudioManager audioManager) 
    {
        _audioManager = audioManager;
    }

    private void Awake() 
    {
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _howToPlayButton.onClick.AddListener(OnHowToPlayButtonClick);
        _creditsButton.onClick.AddListener(OnCreditsButtonClick);
        _quitButton.onClick.AddListener(OnQuitButtonClick);
    }
    
    private void OnPlayButtonClick()
    {
        _audioManager.Play(SoundType.TransitionSound);
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE);
    }

    private void OnHowToPlayButtonClick()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        AnimateContents(_howToPlayContentTransform);
    }

    private void OnCreditsButtonClick()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        AnimateContents(_creditsContentTransform);
    }

    private void OnQuitButtonClick()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        AnimateContents(_quitContentTransform);
    }

    private void AnimateContents(RectTransform newContentTransform)
    {
        if(_currentContentTransform != null && _currentContentTransform != newContentTransform)
        {
            _currentContentTransform.DOAnchorPosX(-1000f, _animationDuration).SetEase(Ease.InBack);
        } 

        newContentTransform.DOAnchorPosX(50f, _animationDuration).SetEase(Ease.OutBack);
        _currentContentTransform = newContentTransform;
        _currentContentTransform.SetAsLastSibling();
    }
}
