using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Points d'Attaque")]
    public Transform attackPoint;
    public Transform attackPointAir;
    public Transform attackPointCrouch;

    [Header("Réglages Impact")]
    public float knockbackForce = 5f;
    public float knockbackLift = 2f;

    private Transform currentAttackPoint;
    private bool hasHitThisAttack;

    private PlayerConfig config;
    private PlayerInputHandler input;
    private PlayerState state;
    private PlayerController controller;

    private float attackTimer;
    private float hitMoment; 

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

        // Attack now
        if (state.isAttacking)
        {
            attackTimer -= Time.deltaTime;

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
            Manequin manequin = enemy.GetComponentInParent<Manequin>();

            if (health == null && manequin == null) continue;

            if (health != null)
            {
                if (health.gameObject == this.gameObject) continue;

                if (health.playerID == config.playerNumber) continue;
                health.TakeDamage(config.attackDamage);
            }
            
            if (manequin != null)
            {
                manequin.TakeDamage(config.attackDamage);
            }

            PlayerRage myRage = GetComponent<PlayerRage>();
            if (myRage != null)
            {
                myRage.AddRage(config.rageGainPerHit);
            }

            Rigidbody2D enemyRb = enemy.GetComponentInParent<Rigidbody2D>();
            
            if (enemyRb != null)
            {
                float directionX = state.facingRight ? 1f : -1f;
                Vector2 knockbackDir = new Vector2(directionX * knockbackForce, knockbackLift);
                enemyRb.linearVelocity = Vector2.zero; 
                enemyRb.AddForce(knockbackDir, ForceMode2D.Impulse);
            }

            break; 
        }
    }

    void OnDrawGizmosSelected()
    {
        
        if (config == null) config = GetComponent<PlayerConfig>();
        if (config == null) return;

        Gizmos.color = Color.red;
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, config.attackRange);
        if (attackPointAir != null) Gizmos.DrawWireSphere(attackPointAir.position, config.attackRange);
        if (attackPointCrouch != null) Gizmos.DrawWireSphere(attackPointCrouch.position, config.attackRange);
    }
}