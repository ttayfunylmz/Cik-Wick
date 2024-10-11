using UnityEngine;

public class CatStateController : MonoBehaviour
{
    [SerializeField] private CatState _currentCatState = CatState.Idle;

    private void Start() 
    {
        ChangeState(CatState.Running);
    }

    public void ChangeState(CatState newCatState)
    {
        if(_currentCatState == newCatState) { return; }

        _currentCatState = newCatState;
    }

    public CatState GetCurrentState()
    {
        return _currentCatState;
    }
}
