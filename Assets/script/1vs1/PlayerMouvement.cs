using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    
    private PlayerConfig config;
    private PlayerInputHandler input; 
    private PlayerState state;

    // Collider data
    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    // Gestion du temps du bouclier
    private float currentShieldTimer = 0f;
    private float burnoutTimer = 0f;
    private bool isBurnedOut = false;

    public void Init(PlayerConfig pc, PlayerInputHandler pi, PlayerState ps)
    {
        config = pc;
        input = pi;
        state = ps;

        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 
        col = GetComponent<BoxCollider2D>();
        
        if (col != null)
        {
            standingColliderSize = col.size;
            standingColliderOffset = col.offset;
        }
    }

    public void HandleMovement()
    {
        // --- 1. Ground Check ---
        if (config.groundCheck != null)
            state.isGrounded = Physics2D.OverlapCircle(config.groundCheck.position, config.groundCheckRadius, config.groundLayer);

        // --- 2. Mise à jour des états (Crouch & Shield) ---
        state.isCrouching = input.CrouchHeld && state.isGrounded;

        // Logique du bouclier
        HandleShieldLogic();

        // --- 3. Déplacement Horizontal ---
        if (state.isCrouching || state.isShielding)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            state.isMoving = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(input.MoveInput.x * config.moveSpeed, rb.linearVelocity.y);
            state.isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        }

        // --- 4. GESTION DU FLIP (Rotation) ---
        // On ne regarde plus l'Input, mais la position de l'ennemi
        HandleFlip();

        // --- 5. Saut ---
        if (input.JumpTriggered && state.isGrounded && !state.isCrouching && !state.isShielding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            state.isGrounded = false;
        }
        input.JumpTriggered = false;

        // --- 6. Gestion Collider Crouch/Stand ---
        if (state.isCrouching) ApplyCrouchCollider();
        else ApplyStandingCollider();
    }

    // Nouvelle fonction dédiée au retournement
    void HandleFlip()
    {
        if (config.opponentTransform == null) return;

        // On calcule la différence de position X entre l'ennemi et nous
        float xDiff = config.opponentTransform.position.x - transform.position.x;

        // Si l'ennemi est à droite (xDiff > 0) et qu'on regarde à gauche (!facingRight)
        if (xDiff > 0 && !state.facingRight)
        {
            Flip();
        }
        // Si l'ennemi est à gauche (xDiff < 0) et qu'on regarde à droite (facingRight)
        else if (xDiff < 0 && state.facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        state.facingRight = !state.facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void HandleShieldLogic()
    {
        if (isBurnedOut)
        {
            state.isShielding = false;
            burnoutTimer -= Time.deltaTime;
            if (burnoutTimer <= 0)
            {
                isBurnedOut = false;
                currentShieldTimer = 0f;
            }
            return;
        }

        if (input.ShieldPressed)
        {
            currentShieldTimer += Time.deltaTime;
            if (currentShieldTimer >= config.maxShieldTime)
            {
                isBurnedOut = true;
                state.isShielding = false;
                burnoutTimer = config.shieldBurnOutCooldown;
            }
            else
            {
                state.isShielding = true;
            }
        }
        else
        {
            state.isShielding = false;
            if (currentShieldTimer > 0)
            {
                currentShieldTimer -= Time.deltaTime * 2f; 
                if (currentShieldTimer < 0) currentShieldTimer = 0;
            }
        }
    }

    void ApplyCrouchCollider()
    {
        if (col == null) return;
        float bottomY = standingColliderOffset.y - standingColliderSize.y / 2f;
        col.size = config.crouchColliderSize;
        col.offset = new Vector2(standingColliderOffset.x, bottomY + config.crouchColliderSize.y / 2f);
    }

    void ApplyStandingCollider()
    {
        if (col == null) return;
        col.size = standingColliderSize;
        col.offset = standingColliderOffset;
    }
}