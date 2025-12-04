using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;

    [Header("---- IMPORTANT ----")]
    public SpriteRenderer visualRenderer;

    [Header("États")]
    public bool CanMove = true;
    public bool CanAttack = true;

    [Header("Mouvement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Contrôles")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Effets Visuels (Attaque)")]
    public GameObject attackEffectPrefab; 
    public Sprite[] attackSprites;        
    public float frameRate = 0.05f;       
    [Range(0.5f, 5f)] public float effectSizeMultiplier = 1f; 
    public Vector2 effectOffset; 

    [Header("Accroupissement & Sol")]
    [SerializeField] private Vector2 standingSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 0.5f);
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim; // On réutilise l'animator standard pour ne rien casser
    
    [SerializeField] private bool isCrouching;
    [SerializeField] bool Grounded;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>(); // Récupération automatique

        if (visualRenderer == null) 
        {
            visualRenderer = GetComponent<SpriteRenderer>();
            if (visualRenderer == null) visualRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        col.size = standingSize;
    }

    void Update()
    {
        // Vérification du sol
        if (groundCheck != null)
            Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!CanMove) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); return; }

        // --- ATTAQUE ---
        if (Input.GetKeyDown(attackKey))
        {
            if (CanAttack && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // --- MOUVEMENT ---
        float x = Input.GetAxisRaw("Horizontal");

        // --- ACCROUPISSEMENT ---
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

        // Application de la vitesse
        float currentSpeed = isCrouching ? crouchSpeed : speed;
        rb.linearVelocity = new Vector2(x * currentSpeed, rb.linearVelocity.y);

        // --- SAUT ---
        if (Input.GetKeyDown(jumpKey) && Grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // --- DIRECTION DU REGARD ---
        if (x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // --- ANIMATION STANDARD ---
        // On envoie la vitesse à l'Animator pour qu'il gère Idle/Run tout seul
        if (anim != null && visualRenderer.enabled)
        {
            anim.SetFloat("Speed", Mathf.Abs(x));
            anim.SetBool("IsJumping", !Grounded);
            anim.SetBool("IsCrouching", isCrouching);
        }
    }

    void Attack()
    {
        StartCoroutine(PerformAttackSequence());
    }

    IEnumerator PerformAttackSequence()
    {
        // 1. Dégâts immédiats (ou retardés selon tes besoins)
        if (attackPoint != null) Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 2. Si on a des effets visuels configurés
        if (attackEffectPrefab != null && attackSprites.Length > 0)
        {
            // On cache le sprite du joueur (l'Animator tourne toujours mais on ne voit pas le perso)
            if (visualRenderer != null) visualRenderer.enabled = false;

            // Création de l'effet
            GameObject effet = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
            effet.transform.SetParent(this.transform);
            effet.transform.localScale = new Vector3(effectSizeMultiplier, effectSizeMultiplier, 1f);
            effet.transform.localPosition = effectOffset;

            SpriteRenderer srEffet = effet.GetComponent<SpriteRenderer>();
            if (srEffet != null && visualRenderer != null) srEffet.sortingOrder = visualRenderer.sortingOrder + 1;

            // Animation de l'effet frame par frame
            foreach (Sprite frame in attackSprites)
            {
                if (srEffet != null) srEffet.sprite = frame;
                yield return new WaitForSeconds(frameRate);
            }

            // Nettoyage
            Destroy(effet);
            
            // On réaffiche le joueur
            if (visualRenderer != null) visualRenderer.enabled = true;
        }
        else
        {
            // Sécurité : si pas d'effets, on attend juste un peu pour ne pas spammer
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag("Grounded")) Grounded = true; }
    private void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject.CompareTag("Grounded")) Grounded = false; }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
        if (attackPoint != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
    }
}