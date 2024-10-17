using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuContentUI : MonoBehaviour
{
    [Header("Quit Content References")]
    [SerializeField] private RectTransform _quitContentTransform;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private void Awake() 
    {
        _yesButton.onClick.AddListener(OnYesButtonClick);
        _noButton.onClick.AddListener(OnNoButtonClick);    
    }

    private void OnYesButtonClick()
    {
        Application.Quit();
        Debug.Log("Quitting the Game..");
    }

    private void OnNoButtonClick()
    {
        _quitContentTransform.DOAnchorPosX(-1000f, _animationDuration).SetEase(Ease.InBack);
    }
}
