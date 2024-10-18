using UnityEngine;

public class ObjectInteractable : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private string _interactText;
    [SerializeField] private string _speechText;

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
        Debug.Log(_speechText);
    }
}
