using UnityEngine;

public class SpatulaBooster : MonoBehaviour, IBoostable
{
    [Header("References")]
    [SerializeField] private Animator _spatulaAnimator;
    [SerializeField] private GameObject _boostParticlesPrefab;

    [Header("Settings")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _boostParticlesDestroyDuration = 2f;

    private bool _isActivated;

    public void Boost(PlayerController playerController)
    {
        if(_isActivated) { return; }

        PlayBoostAnimation();
        Rigidbody playerRigidbody = playerController.GetPlayerRigidbody();
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(transform.forward * _jumpForce, ForceMode.Impulse);
        _isActivated = true;
        Invoke(nameof(ResetActivation), 0.2f);
    }

    public void PlayBoostAnimation()
    {
        _spatulaAnimator.SetTrigger("IsSpatulaJumping");
    }

    public void PlayBoostParticle(Transform playerTransform)
    {
        Vector3 offset = new Vector3(0f, 0.7f, 0f);

        GameObject particleInstance = 
            Instantiate(_boostParticlesPrefab, playerTransform.position + offset, _boostParticlesPrefab.transform.rotation);

        particleInstance.transform.parent = playerTransform;

        Destroy(particleInstance, _boostParticlesDestroyDuration);
    }

    private void ResetActivation()
    {
        _isActivated = false;
    }
}
