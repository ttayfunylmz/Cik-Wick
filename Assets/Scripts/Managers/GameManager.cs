using System;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private int _currentEggCount;
    private int _maxEggCount;

    private EggCounterUI _eggCounterUI;

    [Inject]
    private void ZenjectSetup(EggCounterUI eggCounterUI)
    {
        _eggCounterUI = eggCounterUI;
    }

    private void Start() 
    {
        _maxEggCount = 5;
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
