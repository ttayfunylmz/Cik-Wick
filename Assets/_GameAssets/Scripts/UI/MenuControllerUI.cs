using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControllerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private RectTransform _headerImageTransform;

    [Header("Content References")]
    [SerializeField] private RectTransform _settingsContentTransform;
    [SerializeField] private RectTransform _creditsContentTransform;
    [SerializeField] private RectTransform _quitContentTransform;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private RectTransform _currentContentTransform = null;

    private void Awake() 
    {
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _settingsButton.onClick.AddListener(OnSettingsButtonClick);
        _creditsButton.onClick.AddListener(OnCreditsButtonClick);
        _quitButton.onClick.AddListener(OnQuitButtonClick);
    }
    
    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnSettingsButtonClick()
    {
        AnimateContents(_settingsContentTransform);
    }

    private void OnCreditsButtonClick()
    {
        AnimateContents(_creditsContentTransform);
    }

    private void OpenLinks(string link)
    {
        Application.OpenURL(link);
    }

    private void OnQuitButtonClick()
    {
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
