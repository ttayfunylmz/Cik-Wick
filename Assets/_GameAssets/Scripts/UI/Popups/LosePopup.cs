using System;
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

    [Inject]
    private void ZenjectSetup(TimerUI timerUI)
    {
        _timerUI = timerUI;
    }

    private void OnEnable() 
    {
        SetTimerText();

        _tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);    
    }

    private void OnMainMenuButtonClicked()
    {

    }

    private void OnTryAgainButtonClicked()
    {

    }
    
    public void SetTimerText()
    {
        _timerText.text = _timerUI.GetFinalTime();
    }

    private void OnDisable() 
    {
        _tryAgainButton.onClick.RemoveListener(OnTryAgainButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);    
    }

}
