using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private PlayerState _currentPlayerState = PlayerState.Idle;

    private void Start() 
    {
        ChangeState(PlayerState.Idle);
    }

    public void ChangeState(PlayerState newPlayerState)
    {
        if(_currentPlayerState == newPlayerState) { return; }

        _currentPlayerState = newPlayerState;
    }

    public PlayerState GetCurrentState()
    {
        return _currentPlayerState;
    }
}
