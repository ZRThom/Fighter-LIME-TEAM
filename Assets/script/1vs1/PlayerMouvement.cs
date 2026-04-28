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
        rb.gravityScale = 2.5f; 
        col = GetComponent<BoxCollider2D>();
        
        if (col != null)
        {
            standingColliderSize = col.size;
            standingColliderOffset = col.offset;
        }
    }

    public void HandleMovement()
    {
        // check car bug (override si necessaire au niveau des hit / death)
        if (state.isDead || state.isHit)
        {
            if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            state.isMoving = false;
            return;
        }


        
        //  Ground Check 
        if (config.groundCheck != null)
            state.isGrounded = Physics2D.OverlapCircle(config.groundCheck.position, config.groundCheckRadius, config.groundLayer);

        //  Mise à jour des états (Crouch & Shield) 
        state.isCrouching = input.CrouchHeld && state.isGrounded;

        // Logique du bouclier
        HandleShieldLogic();

        //  Déplacement Horizontal 
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


        HandleFlip();

        // Saut 
        if (input.JumpTriggered && state.isGrounded && !state.isCrouching && !state.isShielding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            state.isGrounded = false;
        }
        input.JumpTriggered = false;

        //  Gestion Collider Crouch/Stand 
        if (state.isCrouching) ApplyCrouchCollider();
        else ApplyStandingCollider();
    }

    void HandleFlip()
    {
        if (config.opponentTransform == null) return;

        float xDiff = config.opponentTransform.position.x - transform.position.x;

        if (xDiff > 0 && !state.facingRight)
        {
            Flip();
        }
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