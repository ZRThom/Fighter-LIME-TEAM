// PlayerController2D amélioré avec :
// - Hitbox sol / air / accroupi
// - Knockback du mannequin
// - Cooldown d'attaque
// - FIX: 1 seul dégât par attaque (plus de spam de textes)

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // <--- AJOUT INDISPENSABLE POUR LES LISTES

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
    public Sprite[] airAttackSprites;
    public Sprite[] crouchAttackSprites;

    public float animSpeed = 0.1f;

    [Header("---- ETATS ----")]
    public bool CanMove = true;
    public bool CanAttack = true;
    [SerializeField] private int health = 100;

    [Header("---- VITESSE ----")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float jumpForce = 7f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("---- COMBAT ----")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private int attackDamage = 5;

    [Header("Hitboxes diverses")]
    public Transform groundAttackPoint;
    public Transform airAttackPoint;
    public Transform crouchAttackPoint;

    public float groundAttackRange = 0.1f;
    public float airAttackRange = 0.12f;
    public float crouchAttackRange = 0.08f;

    public LayerMask enemyLayer;
    public float attackCooldown = 0.3f;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    [Header("---- PHYSIQUE ----")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Vector2 standingSize = new Vector2(1f, 1f);
    public Vector2 crouchSize = new Vector2(1f, 0.5f);

    private Vector2 standingOffset;

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
    private bool attackOnCooldown;

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
    }

    void Update()
    {
        Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!CanMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        isMoving = Mathf.Abs(x) > 0.1f;

        HandleCrouch();
        isRunning = Input.GetKey(runKey) && !isCrouching && isMoving;

        float currentSpeed = isCrouching ? crouchSpeed : isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(x * currentSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(jumpKey) && Grounded && !isCrouching)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (!isAttacking)
            HandleManualAnimation();

        if (Input.GetKeyDown(attackKey) && !isAttacking && !attackOnCooldown)
            StartCoroutine(PerformAttackSequence());
    }

    IEnumerator PerformAttackSequence()
    {
        isAttacking = true;
        attackOnCooldown = true;

        // --- FIX : On crée la liste des ennemis déjà touchés pour CETTE attaque ---
        List<GameObject> enemiesHitThisAttack = new List<GameObject>();

        Sprite[] selectedAttack = attackSprites;

        if (!Grounded && airAttackSprites.Length > 0)
            selectedAttack = airAttackSprites;
        else if (isCrouching && crouchAttackSprites.Length > 0)
            selectedAttack = crouchAttackSprites;

        foreach (var frame in selectedAttack)
        {
            visualRenderer.sprite = frame;
            
            // On envoie la liste à la fonction de détection
            DetectAttackHit(enemiesHitThisAttack);
            
            yield return new WaitForSeconds(animSpeed);
        }

        isAttacking = false;
        currentAnimSet = idleSprites;

        yield return new WaitForSeconds(attackCooldown);
        attackOnCooldown = false;
    }

    // --- FIX : La fonction accepte maintenant la liste en paramètre ---
    void DetectAttackHit(List<GameObject> enemiesHitList)
    {
        Transform point = groundAttackPoint;
        float range = groundAttackRange;

        if (!Grounded)
        {
            point = airAttackPoint;
            range = airAttackRange;
        }
        else if (isCrouching)
        {
            point = crouchAttackPoint;
            range = crouchAttackRange;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, range, enemyLayer);
        foreach (var hit in hits)
        {
            // --- FIX : Si l'ennemi est déjà dans la liste, on l'ignore ---
            if (enemiesHitList.Contains(hit.gameObject)) continue;

            Manequin m = hit.GetComponent<Manequin>();
            if (m != null)
            {
                m.TakeDamage(attackDamage);

                // --- FIX : On ajoute l'ennemi à la liste pour ne plus le retoucher durant cette attaque ---
                enemiesHitList.Add(hit.gameObject);

                Rigidbody2D ennemiRB = hit.GetComponent<Rigidbody2D>();
                if (ennemiRB != null)
                {
                    float direction = transform.localScale.x > 0 ? 1 : -1;
                    ennemiRB.AddForce(new Vector2(direction * knockbackForce, knockbackForce * 0.3f), ForceMode2D.Impulse);
                }
            }
        }
    }

    void HandleManualAnimation()
    {
        Sprite[] target = idleSprites;

        if (isCrouching) target = crouchSprites;
        else if (!Grounded && jumpSprites.Length > 0) target = jumpSprites;
        else if (isMoving) target = isRunning ? runSprites : walkSprites;

        if (target != currentAnimSet)
        {
            currentAnimSet = target;
            currentFrame = 0;
            animTimer = 0;
        }

        animTimer += Time.deltaTime;
        if (animTimer >= animSpeed)
        {
            animTimer = 0;
            currentFrame = (currentFrame + 1) % currentAnimSet.Length;
        }

        visualRenderer.sprite = currentAnimSet[currentFrame];
    }

    void HandleCrouch()
    {
        bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (Grounded && crouchInput)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                ApplyColliderSize(crouchSize);
            }
        }
        else if (isCrouching)
        {
            isCrouching = false;
            ApplyColliderSize(standingSize);
        }
    }

    void ApplyColliderSize(Vector2 targetSize)
    {
        float pivotFactor = 0.5f;
        Vector2 newOffset = new Vector2(standingOffset.x, standingOffset.y - (standingSize.y - targetSize.y) * pivotFactor);
        col.size = targetSize;
        col.offset = newOffset;
    }

    void OnDrawGizmosSelected()
    {
        if (groundAttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundAttackPoint.position, groundAttackRange);
        }
        if (airAttackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(airAttackPoint.position, airAttackRange);
        }
        if (crouchAttackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(crouchAttackPoint.position, crouchAttackRange);
        }
    }
}