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

    [Inject]
    private void ZenjectSetup(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void Collect()
    {
        _gameManager.OnEggCollected();
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
