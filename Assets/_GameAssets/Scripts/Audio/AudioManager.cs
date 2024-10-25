using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sounds")]
    public Sound[] Sounds;
    
    private void Awake() 
    {
        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.AudioClip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.mute = s.Mute;
            s.Source.loop = s.Loop; 
            s.Source.playOnAwake = s.playOnAwake;
        }
    }

    public void Play(SoundType soundType)
    {
        Sound s = Array.Find(Sounds, sound => sound.SoundType == soundType);
        if (s == null)
        {
            Debug.LogWarning($"Sound with type {soundType} not found in AudioManager.");
            return;
        }

        s.Source.Play();
    }

    public void Stop(SoundType soundType)
    {
        Sound s = Array.Find(Sounds, sound => sound.SoundType == soundType);
        if (s == null)
        {
            Debug.LogWarning($"Sound with type {soundType} not found in AudioManager.");
            return;
        }

        s.Source.Stop();
    }

    public void SetSoundEffectsMute(bool isMuted)
    {
        foreach (Sound s in Sounds)
        {
            s.Source.mute = isMuted;
        }
    }
}
