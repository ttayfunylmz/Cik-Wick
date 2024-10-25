using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class EggCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private GameObject _hitParticlesPrefab;

    [Header("Settings")]
    [SerializeField] private float _hitParticlesDestroyDuration;

    private GameManager _gameManager;
    private ChatBubbleUI _chatBubbleUI;
    private CameraShake _cameraShake;

    [Inject]
    private void ZenjectSetup(GameManager gameManager, ChatBubbleUI chatBubbleUI,
        CameraShake cameraShake)
    {
        _gameManager = gameManager;
        _chatBubbleUI = chatBubbleUI;
        _cameraShake = cameraShake;
    }

    public void Collect()
    {
        _gameManager.OnEggCollected();

        int eggCount = _gameManager.GetCurrentEggCount();

        string message = eggCount switch
        {
            1 => "1 FOUND, 4 TO GO!",
            2 => "ALMOST HALFWAY!",
            3 => "2 SIBLINGS LEFT!",
            4 => "ONLY 1 LEFT!",
            _ => "SOMETHING IS WRONG!"
        };

        _chatBubbleUI.PlayChatBubbleAnimation(message);
        _cameraShake.ShakeCamera(0.5f, 0.5f);
        
        Destroy(gameObject);
    }

    public void PlayParticle(Transform playerTransform)
    {
        
    }

    public void PlayHitParticle(Transform playerTransform)
    {
        Vector3 offset = new Vector3(0f, 0.7f, 0f);

        GameObject particleInstance = 
            Instantiate(_hitParticlesPrefab, playerTransform.position + offset, _hitParticlesPrefab.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _hitParticlesDestroyDuration);
    }
}
