using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ButtonHoverController : MonoBehaviour, IPointerEnterHandler
{
    private AudioManager _audioManager;

    [Inject]
    private void ZenjectSetup(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _audioManager.Play(SoundType.ButtonHoverSound);
    }
}
