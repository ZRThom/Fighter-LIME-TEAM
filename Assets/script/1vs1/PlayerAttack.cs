using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Points d'Attaque")]
    public Transform attackPoint;
    public Transform attackPointAir;
    public Transform attackPointCrouch;

    private Transform currentAttackPoint;
    private bool hasHitThisAttack;

    private PlayerConfig config;
    private PlayerInputHandler input;
    private PlayerState state;
    private PlayerController controller;

    private float attackTimer;
    private float hitMoment; // moment où le coup sort

    public void Init(PlayerConfig pc, PlayerInputHandler pi, PlayerState ps, PlayerController ctrl)
    {
        config = pc;
        input = pi;
        state = ps;
        controller = ctrl;
    }

    public void HandleCombat()
    {
        HandleShield();

        // Start Attack
        if (input.AttackTriggered && !state.isAttacking && !state.isShielding)
        {
            StartAttackLogic();
        }
        input.AttackTriggered = false;

        // Attaque en cours
        if (state.isAttacking)
        {
            attackTimer -= Time.deltaTime;

            // 👉 HIT AU BON MOMENT (sans Animation Event)
            if (!hasHitThisAttack && attackTimer <= hitMoment)
            {
                PerformDamage(currentAttackPoint);
                hasHitThisAttack = true;
            }

            if (attackTimer <= 0f)
            {
                state.isAttacking = false;
            }
        }
    }

    void HandleShield()
    {
        if (state.shieldCooldownTimer > 0f)
        {
            state.shieldCooldownTimer -= Time.deltaTime;
            return;
        }

        if (input.ShieldPressed && !state.shieldBurnedOut)
        {
            state.isShielding = true;
            state.shieldTimer += Time.deltaTime;

            if (state.shieldTimer >= config.maxShieldTime)
            {
                state.isShielding = false;
                state.shieldBurnedOut = true;
                state.shieldCooldownTimer = config.shieldBurnOutCooldown;
                state.shieldTimer = 0f;
            }
        }
        else
        {
            state.isShielding = false;
            state.shieldTimer = Mathf.Max(0f, state.shieldTimer - Time.deltaTime);
        }

        if (state.shieldBurnedOut && state.shieldCooldownTimer <= 0f)
            state.shieldBurnedOut = false;
    }

    void StartAttackLogic()
    {
        state.isAttacking = true;
        hasHitThisAttack = false;

        currentAttackPoint = attackPoint;

        var animData = config.characterAnimations[config.playerCharacterIndex];
        Sprite[] currentSprites = animData.attackSprites;
        float speed = config.attackNormalSpeed;

        if (!state.isGrounded && animData.attackAirSprites.Length > 0 && attackPointAir != null)
        {
            currentAttackPoint = attackPointAir;
            currentSprites = animData.attackAirSprites;
            speed = config.attackAirSpeed;
        }
        else if (state.isCrouching && animData.attackCrouchSprites.Length > 0 && attackPointCrouch != null)
        {
            currentAttackPoint = attackPointCrouch;
            currentSprites = animData.attackCrouchSprites;
            speed = config.attackCrouchSpeed;
        }

        attackTimer = speed * currentSprites.Length;

        // 👉 le coup sort à 40% de l’attaque
        hitMoment = attackTimer * 0.6f;

        controller.SetAttackAnim(currentSprites, speed);
    }

    void PerformDamage(Transform point)
    {
        if (point == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            point.position,
            config.attackRange,
            config.enemyLayers
        );

        foreach (Collider2D enemy in hits)
        {
            PlayerHealth health = enemy.GetComponentInParent<PlayerHealth>();
            if (health == null) continue;

            health.TakeDamage(config.attackDamage);
            break; // 1vs1 → un seul adversaire
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, config.attackRange);

        if (attackPointAir != null)
            Gizmos.DrawWireSphere(attackPointAir.position, config.attackRange);

        if (attackPointCrouch != null)
            Gizmos.DrawWireSphere(attackPointCrouch.position, config.attackRange);
    }
}