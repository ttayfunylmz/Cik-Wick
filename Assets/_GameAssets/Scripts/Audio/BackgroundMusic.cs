using UnityEngine;
using Zenject;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetMusicMute(bool isMuted)
    {
        _audioSource.mute = isMuted;
    }
}
