using UnityEngine;

public class PlayerState : MonoBehaviour
{

    public bool isGrounded = true;
    public bool facingRight = true;
    public bool isCrouching;
    public bool isMoving;
    public bool isAttacking;
    public bool isSpecialAttacking;
    public bool isShielding;
    public bool shieldBurnedOut;
    public bool isAI = false;

    [HideInInspector] public float shieldTimer;
    [HideInInspector] public float shieldCooldownTimer;
}