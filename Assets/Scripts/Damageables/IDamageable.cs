using UnityEngine;

public interface IDamageable
{
    void GiveDamage(Rigidbody playerRigidbody, Transform playerVisualTransform);
}
