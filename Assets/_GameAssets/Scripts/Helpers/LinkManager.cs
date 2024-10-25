using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LinkManager : MonoBehaviour
{
    [SerializeField] private string _linkUrl;
    
    private Button _button;

    private AudioManager _audioManager;

    [Inject]
    private void ZenjectSetup(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start() 
    {
        _button.onClick.AddListener(OpenLink);
    }

    public void OpenLink()
    {
        _audioManager.Play(SoundType.ButtonClickSound);
        Application.OpenURL(_linkUrl);
    }
}
