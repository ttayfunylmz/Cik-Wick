using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _fightingParticles;

    [Header("Settings")]
    [SerializeField] private float _delay = 1f;

    private int _currentEggCount;
    private int _maxEggCount;

    private EggCounterUI _eggCounterUI;
    private WinLoseUI _winLoseUI;
    private CatController _catController;

    [Inject]
    private void ZenjectSetup(EggCounterUI eggCounterUI, WinLoseUI winLoseUI, CatController catController)
    {
        _eggCounterUI = eggCounterUI;
        _winLoseUI = winLoseUI;
        _catController = catController;
    }

    private void Start() 
    {
        _maxEggCount = 5;

        _catController.OnCatCatched += CatController_OnCatCatched;
    }

    private void CatController_OnCatCatched(Transform playerTransform)
    {
        StartCoroutine(OnGameOver(playerTransform));
    }

    private IEnumerator OnGameOver(Transform playerTransform)
    {
        yield return new WaitForSeconds(_delay);
        Instantiate(_fightingParticles, playerTransform.position, _fightingParticles.transform.rotation);
        _winLoseUI.OnGameOver();
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount,  _maxEggCount);

        if(_currentEggCount == _maxEggCount)
        {
            _eggCounterUI.SetEggCompleted();
            Debug.Log("You Collected All Eggs!");
        }
    }
}
