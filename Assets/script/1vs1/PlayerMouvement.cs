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

    public float animSpeed = 0.1f;
    public float attackAnimSpeed = 0.08f;
    public float attackDuration = 0.3f;

    // internes animation
    private float animTimer;
    private int currentFrame;
    private Sprite[] currentAnimSet;

    private float attackTimer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool attackPressed;
    private bool isGrounded = true;
    private bool facingRight = true;

    // états locaux
    private bool isCrouching;
    private bool isMoving;
    private bool isAttacking;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (visualRenderer == null)
            visualRenderer = GetComponent<SpriteRenderer>();

        currentAnimSet = idleSprites;
    }

    void Update()
    {
        // Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

        ReadInputs();
        Move();
        Flip(moveInput.x);

        // Saut
        if (jumpPressed && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // Attaque
        if (attackPressed && !isAttacking)
        {
            StartAttack();
        }

        jumpPressed = false;
        attackPressed = false;

        // Timer attaque
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                isAttacking = false;
        }

        // Animation
        if (!isAttacking)
            HandleManualAnimation();
        else
            HandleAttackAnimation();
    }

    // --- INPUTS ---
    void ReadInputs()
    {
        if (playerNumber == 1)
        {
            float h = 0f;
            if (Keyboard.current.aKey.isPressed) h = -1f;
            else if (Keyboard.current.dKey.isPressed) h = 1f;

            moveInput = new Vector2(h, 0f);

            jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            isCrouching = Keyboard.current.sKey.isPressed && isGrounded;
            attackPressed = Mouse.current.leftButton.wasPressedThisFrame;
        }
        else
        {
            float h = 0f;
            if (Keyboard.current.leftArrowKey.isPressed) h = -1f;
            else if (Keyboard.current.rightArrowKey.isPressed) h = 1f;

            moveInput = new Vector2(h, 0f);

            jumpPressed = Keyboard.current.rightShiftKey.wasPressedThisFrame;
            isCrouching = Keyboard.current.downArrowKey.isPressed && isGrounded;
            attackPressed = Mouse.current.rightButton.wasPressedThisFrame;
        }
    }

    // --- MOUVEMENT ---
    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        isMoving = Mathf.Abs(moveInput.x) > 0.1f;
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

    // --- ATTAQUE ---
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

    // --- ANIMATION ---
    void HandleManualAnimation()
    {
        Sprite[] target = idleSprites;

        if (isCrouching && crouchSprites.Length > 0)
            target = crouchSprites;
        else if (!isGrounded && jumpSprites.Length > 0)
            target = jumpSprites;
        else if (isMoving && walkSprites.Length > 0)
            target = walkSprites;

        if (target != currentAnimSet)
        {
            currentAnimSet = target;
            currentFrame = 0;
            animTimer = 0f;
        }

        if (currentAnimSet == null || currentAnimSet.Length == 0)
            return;

        animTimer += Time.deltaTime;
        if (animTimer >= animSpeed)
        {
            animTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimSet.Length;
        }

        visualRenderer.sprite = currentAnimSet[currentFrame];
    }

    void HandleAttackAnimation()
    {
        if (attackSprites == null || attackSprites.Length == 0)
            return;

        animTimer += Time.deltaTime;
        if (animTimer >= attackAnimSpeed)
        {
            animTimer = 0f;
            currentFrame = (currentFrame + 1) % attackSprites.Length;
            visualRenderer.sprite = attackSprites[currentFrame];
        }
    }

    // --- GIZMOS ---
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
