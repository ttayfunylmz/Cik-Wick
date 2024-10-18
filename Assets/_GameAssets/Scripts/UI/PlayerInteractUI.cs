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

    [Inject]
    private void ZenjectSetup(PlayerInteractionController playerInteractionController)
    {
        _playerInteractionController = playerInteractionController;
    }

    private void Update() 
    {
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
