using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // États publics pour lecture par les autres scripts
    public bool isGrounded = true;
    public bool facingRight = true;
    public bool isCrouching;
    public bool isMoving;
    public bool isAttacking;
    public bool isShielding;
    public bool shieldBurnedOut;

    // Timers gérés ici ou dans les composants respectifs
    [HideInInspector] public float shieldTimer;
    [HideInInspector] public float shieldCooldownTimer;
}