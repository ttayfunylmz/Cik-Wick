using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerInteractUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup _interactObjectCanvasGroup;
    [SerializeField] private TMP_Text _interactText;

    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;

    private PlayerInteractionController _playerInteractionController;
    private GameManager _gameManager;

    [Inject]
    private void ZenjectSetup(PlayerInteractionController playerInteractionController,
        GameManager gameManager)
    {
        _playerInteractionController = playerInteractionController;
        _gameManager = gameManager;
    }

    private void Update() 
    {
        if(_gameManager.GetCurrentGameState() != GameState.Play 
            && _gameManager.GetCurrentGameState() != GameState.Resume) { return; }

        if(_playerInteractionController.GetInteractableObject() != null)
        {
            ShowInteractObject(_playerInteractionController.GetInteractableObject());
        }
        else
        {
            HideInteractObject();
        } 
    }

    private void ShowInteractObject(IInteractable interactable)
    {
        _interactObjectCanvasGroup.DOFade(1f, _fadeDuration);
        _interactText.text = interactable.GetInteractText();
    }

    private void HideInteractObject()
    {
        _interactObjectCanvasGroup.DOFade(0f, _fadeDuration);
    }
}
