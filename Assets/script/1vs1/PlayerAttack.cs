using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Points d'Attaque")]
    public Transform attackPoint;
    public Transform attackPointAir;
    public Transform attackPointCrouch;

    private PlayerConfig config;
    private PlayerInputHandler input; // MODIFIÉ ICI (était PlayerInput)
    private PlayerState state;
    private PlayerController controller; 

    private float attackTimer;

    // MODIFIÉ ICI : Le paramètre 'pi' est maintenant de type PlayerInputHandler
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

        // Timer Attaque en cours
        if (state.isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                state.isAttacking = false;
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
        Transform currentPoint = attackPoint;
        var animData = config.characterAnimations[config.playerCharacterIndex];
        Sprite[] currentSprites = animData.attackSprites;
        float speed = config.attackNormalSpeed;

        if (!state.isGrounded && animData.attackAirSprites.Length > 0 && attackPointAir != null)
        {
            currentPoint = attackPointAir;
            currentSprites = animData.attackAirSprites;
            speed = config.attackAirSpeed;
        }
        else if (state.isCrouching && animData.attackCrouchSprites.Length > 0 && attackPointCrouch != null)
        {
            currentPoint = attackPointCrouch;
            currentSprites = animData.attackCrouchSprites;
            speed = config.attackCrouchSpeed;
        }

        attackTimer = speed * currentSprites.Length;
        
        // Lancer l'anim via le controller
        controller.SetAttackAnim(currentSprites, speed);
        
        PerformDamage(currentPoint);
    }

    void PerformDamage(Transform point)
    {
        if (point == null) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, config.attackRange, config.enemyLayers);
        foreach (Collider2D enemy in hits)
        {
            PlayerHealth health = enemy.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(config.attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, 0.5f); 
        if (attackPointAir != null) Gizmos.DrawWireSphere(attackPointAir.position, 0.5f);
        if (attackPointCrouch != null) Gizmos.DrawWireSphere(attackPointCrouch.position, 0.5f);
    }
}