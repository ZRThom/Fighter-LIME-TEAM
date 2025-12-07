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
    public Sprite[] jumpSprites;   // <--- AJOUTÉ : Tes images de saut
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

    private Rigidbody2D rb;
    private BoxCollider2D col;
    
    private float animTimer;
    private int currentFrame;
    private Sprite[] currentAnimSet; 
    
    private bool isCrouching;
    private bool isRunning; 
    private bool Grounded;
    private bool isMoving;
    
    // Variable pour savoir si on tape (sert uniquement à bloquer l'anim de marche)
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
        if (visualRenderer == null) visualRenderer = GetComponent<SpriteRenderer>();
        currentAnimSet = idleSprites;
    }

    void Update()
    {
        // --- 1. DETECTION SOL ---
        if (groundCheck != null)
            Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!CanMove) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); return; }

        // --- 2. INPUTS & MOUVEMENT ---
        float x = Input.GetAxisRaw("Horizontal");
        isMoving = Mathf.Abs(x) > 0.1f;

        // Accroupissement
        if (Grounded && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            isCrouching = true;
            col.size = crouchSize;
        }
        else
        {
            isCrouching = false;
            col.size = standingSize;
        }

        // Course
        isRunning = Input.GetKey(runKey) && !isCrouching && isMoving;

        // Calcul Vitesse
        float currentSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isRunning) currentSpeed = runSpeed;
        else currentSpeed = walkSpeed;

        // Application du mouvement
        rb.linearVelocity = new Vector2(x * currentSpeed, rb.linearVelocity.y);

        // Saut
        if (Input.GetKeyDown(jumpKey) && Grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Direction (Flip)
        if (x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // --- 3. GESTION DE L'ANIMATION ---
        if (!isAttacking) 
        {
            HandleManualAnimation();
        }

        // --- 4. DÉCLENCHEMENT ATTAQUE ---
        if (Input.GetKeyDown(attackKey) && CanAttack && !isAttacking)
        {
            StartCoroutine(PerformAttackSequence());
        }

        // --- 5. LOGIQUE JEU (Santé, Score, Hitbox) ---
        if (isAttacking && visualRenderer != null)
        {
            // Note: J'ai commenté ces lignes car elles causent des erreurs sans le script Ennemi
            // handleAttackHitbox(); 
            // health.ennemie -= 1; 
        }

        if (health <= 0)
        {
            Debug.Log("Player is dead!");
        }

        // Note : J'ai déplacé ce log car dans Update il s'affiche 60 fois par seconde
        if (score >= 3)
        {
            Debug.Log("Player wins the game!");
        }
    }

    void HandleManualAnimation()
    {
        if (visualRenderer == null) return;

        Sprite[] targetAnimSet = idleSprites; 

        // --- LOGIQUE DE PRIORITÉ MISE À JOUR ---
        if (isCrouching) 
        {
            targetAnimSet = crouchSprites;
        }
        else if (!Grounded) // <--- AJOUTÉ : Si on n'est pas au sol -> SAUT
        {
            if (jumpSprites != null && jumpSprites.Length > 0)
                targetAnimSet = jumpSprites;
        }
        else if (isMoving)
        {
            if (isRunning) targetAnimSet = runSprites;
            else targetAnimSet = walkSprites;
        }
        else 
        {
            targetAnimSet = idleSprites;
        }
        // ---------------------------------------

        if (targetAnimSet != currentAnimSet)
        {
            currentAnimSet = targetAnimSet;
            currentFrame = 0;
            animTimer = 0;
        }

        if (currentAnimSet != null && currentAnimSet.Length > 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animSpeed)
            {
                animTimer = 0;
                currentFrame++;
                if (currentFrame >= currentAnimSet.Length) currentFrame = 0; 
            }
            
            if (currentFrame < currentAnimSet.Length)
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
}