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

    public void Init(PlayerConfig pc, PlayerInputHandler pi, PlayerState ps)
    {
        config = pc;
        input = pi;
        state = ps;

        rb = GetComponent<Rigidbody2D>();
        // Sécurité : on s'assure que la rotation est figée (même si fait dans l'inspecteur)
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
        state.isShielding = input.ShieldPressed; 

        // --- 3. Déplacement Horizontal ---
        // SI on est accroupi OU en train de parer (Shield) : ON NE BOUGE PAS
        if (state.isCrouching || state.isShielding)
        {
            // On met la vitesse X à 0, mais on conserve la gravité (Y)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            state.isMoving = false;
        }
        else
        {
            // SINON : Mouvement normal
            rb.linearVelocity = new Vector2(input.MoveInput.x * config.moveSpeed, rb.linearVelocity.y);
            state.isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

            // Flip (Seulement autorisé si on peut bouger)
            if ((input.MoveInput.x > 0 && !state.facingRight) || (input.MoveInput.x < 0 && state.facingRight))
            {
                state.facingRight = !state.facingRight;
                Vector3 scale = transform.localScale;
                scale.x *= -1f;
                transform.localScale = scale;
            }
        }

        // --- 4. Saut ---
        // On vérifie qu'on ne shield pas et qu'on ne crouch pas avant de sauter
        if (input.JumpTriggered && state.isGrounded && !state.isCrouching && !state.isShielding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            state.isGrounded = false;
        }
        input.JumpTriggered = false; // Reset du trigger saut

        // --- 5. Gestion Collider Crouch/Stand ---
        if (state.isCrouching) ApplyCrouchCollider();
        else ApplyStandingCollider();
    }

    void ApplyCrouchCollider()
    {
        if (col == null) return;
        
        // Calcul pour garder les pieds au sol quand la hitbox rétrécit
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