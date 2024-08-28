using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;

    private PlayerController _playerController;
    private StateController _stateController;

    [Inject]
    private void ZenjectSetup(PlayerController playerController, StateController stateController)
    {
        _playerController = playerController;
        _stateController = stateController;
    }

    private void Start() 
    {
        _playerController.OnPlayerJumped += PlayerController_OnPlayerJumped;    
    }

    private void Update() 
    {
        SetPlayerAnimations();
    }

    private void PlayerController_OnPlayerJumped()
    {
        _playerAnimator.SetBool(Consts.Animations.IS_JUMPING, true);
        Invoke(nameof(ResetJumping), 0.5f);
    }

    private void ResetJumping()
    {
        _playerAnimator.SetBool(Consts.Animations.IS_JUMPING, false);
    }

    private void SetPlayerAnimations()
    {
        var currentState = _stateController.GetCurrentState();

        switch (currentState)
        {
            case PlayerState.Idle:
                _playerAnimator.SetBool(Consts.Animations.IS_SLIDING, false);
                _playerAnimator.SetBool(Consts.Animations.IS_MOVING, false);
                break;
            case PlayerState.Move:
                _playerAnimator.SetBool(Consts.Animations.IS_SLIDING, false);
                _playerAnimator.SetBool(Consts.Animations.IS_MOVING, true);
                break;
            case PlayerState.SlideIdle:
                _playerAnimator.SetBool(Consts.Animations.IS_SLIDING, true);
                _playerAnimator.SetBool(Consts.Animations.IS_SLIDING_ACTIVE, false);
                break;
            case PlayerState.Slide:
                _playerAnimator.SetBool(Consts.Animations.IS_SLIDING, true);
                _playerAnimator.SetBool(Consts.Animations.IS_SLIDING_ACTIVE, true);
                break;
        }
    }
}
