using DG.Tweening;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;

    private PlayerController _playerController;

    private void Awake() 
    {
        _playerController = GetComponent<PlayerController>();    
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
        if(_playerController.GetMovementDirection() != Vector3.zero)
        {
            _playerAnimator.SetBool(Consts.Animations.IS_MOVING, true);
        }
        else
        {
            _playerAnimator.SetBool(Consts.Animations.IS_MOVING, false);
        }
    }
}
