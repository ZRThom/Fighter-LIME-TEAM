using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;

    [Header("---- VISUELS ----")]
    public SpriteRenderer visualRenderer;
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public Sprite[] runSprites;
    public Sprite[] crouchSprites;
    public Sprite[] jumpSprites;
    public Sprite[] attackSprites;

    [Tooltip("Temps entre chaque image")]
    public float animSpeed = 0.1f;

    [Header("---- ÉTATS ----")]
    public bool CanMove = true;
    public bool CanAttack = true;
    [SerializeField] private int health = 100;
    [SerializeField] private int score = 0;
    [SerializeField] private int maxHealth = 100;

    [Header("---- REGLAGES VITESSE ----")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float jumpForce = 7f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("---- COMBAT & PHYSIQUE ----")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private int attackDamage = 5;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 standingSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 0.5f);

    private Vector2 standingOffset; // Stocke l’offset debout

    private Rigidbody2D rb;
    private BoxCollider2D col;

    private float animTimer;
    private int currentFrame;
    private Sprite[] currentAnimSet;

    private bool isCrouching;
    private bool isRunning;
    private bool Grounded;
    private bool isMoving;
    private bool isAttacking;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        if (visualRenderer == null)
            visualRenderer = GetComponent<SpriteRenderer>();

        currentAnimSet = idleSprites;

        standingOffset = col.offset;
        Debug.Log("Box collider trouvé sur : " + col.gameObject.name);
    }

    void Update()
    {
        // --- 1. DETECTION SOL ---
        if (groundCheck != null)
            Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!CanMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // --- 2. INPUTS & MOUVEMENT ---
        float x = Input.GetAxisRaw("Horizontal");
        isMoving = Mathf.Abs(x) > 0.1f;

        HandleCrouch(); // Gestion accroupissement

        // Course
        isRunning = Input.GetKey(runKey) && !isCrouching && isMoving;

        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isRunning) currentSpeed = runSpeed;

        rb.linearVelocity = new Vector2(x * currentSpeed, rb.linearVelocity.y);

        // Saut
        if (Input.GetKeyDown(jumpKey) && Grounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Flip direction
        if (x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Animation
        if (!isAttacking)
            HandleManualAnimation();

        // Attaque
        if (Input.GetKeyDown(attackKey) && CanAttack && !isAttacking)
            StartCoroutine(PerformAttackSequence());

        if (health <= 0)
            Debug.Log("Player is dead!");

        if (score >= 3)
            Debug.Log("Player wins the game!");
    }

    // --- GESTION DU CROUCH ---
    void HandleCrouch()
    {
        bool crouchInput =
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow);

        if (Grounded && crouchInput)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                ApplyColliderSize(crouchSize);
            }
        }
        else
        {
            if (isCrouching)
            {
                // Vérifie s'il n'y a pas d'obstacle au-dessus pour se relever
                Vector2 checkPos = (Vector2)transform.position + Vector2.up * (standingSize.y / 2f);
                Collider2D hit = Physics2D.OverlapBox(
                    checkPos,
                    new Vector2(standingSize.x * 0.9f, standingSize.y - crouchSize.y),
                    0f,
                    groundLayer
                );

                if (hit == null)
                {
                    isCrouching = false;
                    ApplyColliderSize(standingSize);
                }
            }
        }
    }

    void ApplyColliderSize(Vector2 targetSize)
    {
        float pivotFactor = 0.5f; // 0.5 si pivot centré, 0 si pivot en bas

        Vector2 newOffset = new Vector2(
            standingOffset.x,
            standingOffset.y - (standingSize.y - targetSize.y) * pivotFactor
        );

        col.size = targetSize;
        col.offset = newOffset;

        // Mise à jour visuel pour plus de précision
        if (isCrouching && crouchSprites.Length > 0)
            visualRenderer.sprite = crouchSprites[0];
    }

    void HandleManualAnimation()
    {
        if (visualRenderer == null) return;

        Sprite[] targetAnimSet = idleSprites;

        if (isCrouching)
            targetAnimSet = crouchSprites;
        else if (!Grounded && jumpSprites != null && jumpSprites.Length > 0)
            targetAnimSet = jumpSprites;
        else if (isMoving)
            targetAnimSet = isRunning ? runSprites : walkSprites;
        else
            targetAnimSet = idleSprites;

        if (targetAnimSet != currentAnimSet)
        {
            currentAnimSet = targetAnimSet;
            currentFrame = 0;
            animTimer = 0;
        }

        if (currentAnimSet.Length > 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animSpeed)
            {
                animTimer = 0;
                currentFrame++;
                if (currentFrame >= currentAnimSet.Length)
                    currentFrame = 0;
            }

            visualRenderer.sprite = currentAnimSet[currentFrame];
        }
    }

    IEnumerator PerformAttackSequence()
    {
        isAttacking = true;

        if (attackSprites != null && attackSprites.Length > 0)
        {
            foreach (Sprite frame in attackSprites)
            {
                visualRenderer.sprite = frame;
                yield return new WaitForSeconds(animSpeed);
            }
        }

        isAttacking = false;

        currentAnimSet = idleSprites;
        animTimer = animSpeed;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        Debug.Log("Player took " + damage + " damage. Current health: " + health);
    }
}
