using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Réglages Joueur")]
    public int playerNumber = 1;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Attaque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;

    [Header("Bouclier")]
    public float maxShieldTime = 15f;
    public float shieldBurnOutCooldown = 30f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    [Header("---- VISUELS ----")]
    public SpriteRenderer visualRenderer;
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public Sprite[] jumpSprites;
    public Sprite[] crouchSprites;
    public Sprite[] attackSprites;
    public Sprite[] shieldSprites;

    public float animSpeed = 0.1f;
    public float attackAnimSpeed = 0.08f;
    public float attackDuration = 0.3f;

    private float animTimer;
    private int currentFrame;
    private Sprite[] currentAnimSet;

    private float attackTimer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool attackPressed;
    private bool shieldPressed;
    private bool isGrounded = true;
    private bool facingRight = true;

    private bool isCrouching;
    private bool isMoving;
    private bool isAttacking;

    // ===== SHIELD =====
    private bool isShielding;
    private float shieldTimer;
    private float shieldCooldownTimer;
    private bool shieldBurnedOut;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (visualRenderer == null)
            visualRenderer = GetComponentInChildren<SpriteRenderer>();

        currentAnimSet = idleSprites;
    }

    void Update()
    {
        // Ground check
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        ReadInputs();
        Move();
        Flip(moveInput.x);

        // Jump
        if (jumpPressed && isGrounded && !isCrouching && !isShielding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // Attack
        if (attackPressed && !isAttacking && !isShielding)
        {
            StartAttack();
        }

        // Shield logic
        HandleShield();

        jumpPressed = false;
        attackPressed = false;
        shieldPressed = false;

        // Attack timer
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                isAttacking = false;
        }

        // Animation
        if (isShielding)
            HandleShieldAnimation();
        else if (!isAttacking)
            HandleManualAnimation();
        else
            HandleAttackAnimation();
    }

    void ReadInputs()
    {
        float h = 0f;

        if (playerNumber == 1)
        {
            if (Keyboard.current.aKey.isPressed) h = -1f;
            else if (Keyboard.current.dKey.isPressed) h = 1f;

            jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            isCrouching = Keyboard.current.sKey.isPressed && isGrounded;
            attackPressed = Mouse.current.leftButton.wasPressedThisFrame;
            shieldPressed = Keyboard.current.eKey.isPressed; // Player 1 bouclier
        }
        else
        {
            if (Keyboard.current.leftArrowKey.isPressed) h = -1f;
            else if (Keyboard.current.rightArrowKey.isPressed) h = 1f;

            jumpPressed = Keyboard.current.rightShiftKey.wasPressedThisFrame;
            isCrouching = Keyboard.current.downArrowKey.isPressed && isGrounded;
            attackPressed = Mouse.current.rightButton.wasPressedThisFrame;
            shieldPressed = Keyboard.current.rKey.isPressed; // Player 2 bouclier
        }

        moveInput = new Vector2(h, 0f);
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    }

    void Flip(float horizontal)
    {
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    // ===== SHIELD =====
    void HandleShield()
    {
        if (shieldCooldownTimer > 0f)
        {
            shieldCooldownTimer -= Time.deltaTime;
            return;
        }

        if (shieldPressed && !shieldBurnedOut)
        {
            isShielding = true;
            shieldTimer += Time.deltaTime;

            if (shieldTimer >= maxShieldTime)
            {
                // Burn out
                isShielding = false;
                shieldBurnedOut = true;
                shieldCooldownTimer = shieldBurnOutCooldown;
                shieldTimer = 0f;
            }
        }
        else
        {
            isShielding = false;
            shieldTimer = Mathf.Max(0f, shieldTimer - Time.deltaTime);
        }

        if (shieldBurnedOut && shieldCooldownTimer <= 0f)
        {
            shieldBurnedOut = false;
        }
    }

    public bool IsShieldActive()
    {
        return isShielding;
    }

    // ===== ATTACK =====
    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        currentAnimSet = attackSprites;
        currentFrame = 0;
        animTimer = 0f;

        Attack();
    }

    void Attack()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            PlayerHealth health = enemy.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(attackDamage);
        }
    }

    // ===== ANIMATIONS =====
    void HandleManualAnimation()
    {
        Sprite[] target = idleSprites;

        if (isCrouching && crouchSprites.Length > 0)
            target = crouchSprites;
        else if (!isGrounded && jumpSprites.Length > 0)
            target = jumpSprites;
        else if (isMoving && walkSprites.Length > 0)
            target = walkSprites;

        SwitchAnim(target, animSpeed);
    }

    void HandleAttackAnimation()
    {
        SwitchAnim(attackSprites, attackAnimSpeed);
    }

    void HandleShieldAnimation()
    {
        if (shieldSprites != null && shieldSprites.Length > 0)
            SwitchAnim(shieldSprites, animSpeed);
    }

    void SwitchAnim(Sprite[] target, float speed)
    {
        if (target == null || target.Length == 0) return;

        if (currentAnimSet != target)
        {
            currentAnimSet = target;
            currentFrame = 0;
            animTimer = 0f;
        }

        animTimer += Time.deltaTime;
        if (animTimer >= speed)
        {
            animTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimSet.Length;
        }

        visualRenderer.sprite = currentAnimSet[currentFrame];
    }
}
