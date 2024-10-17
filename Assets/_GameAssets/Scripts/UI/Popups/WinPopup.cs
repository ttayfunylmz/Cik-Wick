using System;
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

    private TimerUI _timerUI;

    [Inject]
    private void ZenjectSetup(TimerUI timerUI)
    {
        _timerUI = timerUI;
    }

    private void OnEnable() 
    {
        SetTimerText();

        _oneMoreButton.onClick.AddListener(OnOneMoreButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnMainMenuButtonClicked()
    {

    }

    private void OnOneMoreButtonClicked()
    {

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