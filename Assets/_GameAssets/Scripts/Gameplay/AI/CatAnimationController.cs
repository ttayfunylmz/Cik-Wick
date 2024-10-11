using UnityEngine;

public class CatAnimationController : MonoBehaviour
{
    private Animator _catAnimator;
    private CatStateController _catStateController;

    private void Awake() 
    {
        _catAnimator = GetComponent<Animator>();
        _catStateController = GetComponent<CatStateController>();    
    }

    private void Update() 
    {
        SetCatAnimations();    
    }

    private void SetCatAnimations()
    {
        var currentCatState = _catStateController.GetCurrentState();

        switch (currentCatState)
        {
            case CatState.Idle:
                _catAnimator.SetBool(Consts.CatAnimations.IS_IDLING, true);
                _catAnimator.SetBool(Consts.CatAnimations.IS_RUNNING, false);
                break;
            case CatState.Running:
                _catAnimator.SetBool(Consts.CatAnimations.IS_IDLING, false);
                _catAnimator.SetBool(Consts.CatAnimations.IS_RUNNING, true);
                break;
        }
    }
}
