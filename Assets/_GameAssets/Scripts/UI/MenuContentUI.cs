using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuContentUI : MonoBehaviour
{
    [Header("Quit Content References")]
    [SerializeField] private RectTransform _quitContentTransform;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private AudioManager _audioManager;

    [Inject]
    private void ZenjectSetup(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake() 
    {
        _yesButton.onClick.AddListener(OnYesButtonClick);
        _noButton.onClick.AddListener(OnNoButtonClick);    
    }

    private void OnYesButtonClick()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        Application.Quit();
        Debug.Log("Quitting the Game..");
    }

    private void OnNoButtonClick()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        _quitContentTransform.DOAnchorPosX(-1000f, _animationDuration).SetEase(Ease.InBack);
    }
}
