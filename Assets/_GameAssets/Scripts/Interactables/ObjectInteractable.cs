using UnityEngine;
using Zenject;

public class ObjectInteractable : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private string _interactText;
    [SerializeField] private string _speechText;

    private ChatBubbleUI _chatBubbleUI;

    [Inject]
    private void ZenjectSetup(ChatBubbleUI chatBubbleUI)
    {
        _chatBubbleUI = chatBubbleUI;
    }

    public string GetInteractText()
    {
        return _interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        _chatBubbleUI.PlayChatBubbleAnimation(_speechText);
    }
}
