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

    // --- NOUVEAU : Variables pour la gestion du temps du bouclier ---
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

        // --- 2. Mise à jour des états (Crouch & Shield LOGIC) ---
        state.isCrouching = input.CrouchHeld && state.isGrounded;

        // Appel de la nouvelle fonction pour gérer le temps du bouclier
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

            if ((input.MoveInput.x > 0 && !state.facingRight) || (input.MoveInput.x < 0 && state.facingRight))
            {
                state.facingRight = !state.facingRight;
                Vector3 scale = transform.localScale;
                scale.x *= -1f;
                transform.localScale = scale;
            }
        }

        // --- 4. Saut ---
        if (input.JumpTriggered && state.isGrounded && !state.isCrouching && !state.isShielding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            state.isGrounded = false;
        }
        input.JumpTriggered = false;

        // --- 5. Gestion Collider Crouch/Stand ---
        if (state.isCrouching) ApplyCrouchCollider();
        else ApplyStandingCollider();
    }

    // --- NOUVELLE FONCTION : LOGIQUE DU BOUCLIER ---
    void HandleShieldLogic()
    {
        // 1. Si le bouclier est en surchauffe (Burnout)
        if (isBurnedOut)
        {
            state.isShielding = false; // Force le bouclier à se désactiver
            burnoutTimer -= Time.deltaTime;

            // Si le temps de recharge est fini
            if (burnoutTimer <= 0)
            {
                isBurnedOut = false;
                currentShieldTimer = 0f; // Reset du timer de bouclier
                Debug.Log("Bouclier réactivé !");
            }
            return; // On arrête là, on ne peut pas utiliser le bouclier
        }

        // 2. Gestion de l'utilisation normale
        if (input.ShieldPressed)
        {
            // On augmente le temps d'utilisation
            currentShieldTimer += Time.deltaTime;

            // Vérifie si on dépasse le temps max
            if (currentShieldTimer >= config.maxShieldTime)
            {
                // SURCHAUFFE !
                isBurnedOut = true;
                state.isShielding = false;
                burnoutTimer = config.shieldBurnOutCooldown;
                Debug.Log("Bouclier en surchauffe (Burnout) !");
            }
            else
            {
                // Utilisation valide
                state.isShielding = true;
            }
        }
        else
        {
            // Si on ne tient pas le bouton
            state.isShielding = false;

            // Optionnel : Récupération du bouclier quand on ne l'utilise pas
            // Ici je le fais descendre 2x plus vite qu'il ne monte pour recharger
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