using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [SerializeField] private TMP_Text _stateText;

    private PlayerState _currentPlayerState = PlayerState.Idle;

    private void Start() 
    {
        ChangeState(PlayerState.Idle);
        SetStateText(PlayerState.Idle);
    }

    public void ChangeState(PlayerState newPlayerState)
    {
        if(_currentPlayerState == newPlayerState) { return; }

        _currentPlayerState = newPlayerState;

        SetStateText(_currentPlayerState);
    }

    public PlayerState GetCurrentState()
    {
        return _currentPlayerState;
    }

    private void SetStateText(PlayerState playerState)
    {
        _stateText.text = playerState.ToString();
    }
}
