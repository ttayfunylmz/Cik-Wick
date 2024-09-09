using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class EggCollectible : MonoBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private GameObject _eggVisualObject;

    [Header("Settings")]
    [SerializeField] private float _firstScaleDuration;
    [SerializeField] private float _secondScaleDuration;
    [SerializeField] private float _moveDuration;

    private GameManager _gameManager;

    [Inject]
    private void ZenjectSetup(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void Collect()
    {
        _gameManager.OnEggCollected();

        transform.DOMoveY(transform.position.y + 1.5f, _moveDuration).SetEase(Ease.Linear);
        transform.DOScale(1.5f, _firstScaleDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(0f, _secondScaleDuration).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                transform.gameObject.SetActive(false);
                _eggVisualObject.SetActive(true);
                _eggVisualObject.transform.DOScale(0.3f, 0.5f).SetEase(Ease.OutBack);
            });
        });
    }

    public void PlayParticle(Transform playerTransform)
    {
        
    }
}
