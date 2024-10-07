using UnityEngine;

public class SpatulaBooster : MonoBehaviour, IBoostable
{
    [Header("References")]
    [SerializeField] private Animator _spatulaAnimator;

    [Header("Settings")]
    [SerializeField] private float _jumpForce;

    public void Boost(PlayerController playerController)
    {
        Rigidbody playerRigidbody = playerController.GetPlayerRigidbody();
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    public void PlayBoostAnimation()
    {
        _spatulaAnimator.SetTrigger("IsSpatulaJumping");
    }
}
