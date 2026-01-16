using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    
    private PlayerConfig config;
    private PlayerInputHandler input; // MODIFIÉ ICI (était PlayerInput)
    private PlayerState state;

    // Collider data
    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    // MODIFIÉ ICI : Le paramètre 'pi' est maintenant de type PlayerInputHandler
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
        // Ground Check
        if (config.groundCheck != null)
            state.isGrounded = Physics2D.OverlapCircle(config.groundCheck.position, config.groundCheckRadius, config.groundLayer);

        // Crouch Check
        state.isCrouching = input.CrouchHeld && state.isGrounded;

        // Déplacement Horizontal
        rb.linearVelocity = new Vector2(input.MoveInput.x * config.moveSpeed, rb.linearVelocity.y);
        state.isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        // Flip
        if ((input.MoveInput.x > 0 && !state.facingRight) || (input.MoveInput.x < 0 && state.facingRight))
        {
            state.facingRight = !state.facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }

        // Saut
        if (input.JumpTriggered && state.isGrounded && !state.isCrouching && !state.isShielding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            state.isGrounded = false;
        }
        input.JumpTriggered = false; // Reset du trigger saut

        // Gestion Collider Crouch/Stand
        if (state.isCrouching) ApplyCrouchCollider();
        else ApplyStandingCollider();
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