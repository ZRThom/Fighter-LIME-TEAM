using UnityEngine;

public class PlayerState : MonoBehaviour
{

    public bool isGrounded = true;
    public bool facingRight = true;
    public bool isCrouching;
    public bool isMoving;
    public bool isAttacking;
    public bool isShielding;
    public bool shieldBurnedOut;

    [HideInInspector] public float shieldTimer;
    [HideInInspector] public float shieldCooldownTimer;
}