using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;

    [Header("---- IMPORTANT ----")]
    [Tooltip("GLISSE ICI l'objet qui contient l'image de ton personnage (le Sprite Renderer)")]
    public SpriteRenderer visualRenderer; // C'est ici qu'on force le lien

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

    [Header("Effets Visuels")]
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
    private Animator anim;
    
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
        anim = GetComponent<Animator>();

        // Sécurité : Si tu as oublié de remplir la case, on essaie de le trouver
        if (visualRenderer == null) 
        {
            visualRenderer = GetComponent<SpriteRenderer>();
            // Si toujours vide, on cherche dans les enfants
            if (visualRenderer == null) visualRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (visualRenderer == null) Debug.LogError("ERREUR CRITIQUE : Je ne trouve pas le SpriteRenderer ! Glisse-le manuellement dans l'inspecteur.");
        }

        col.size = standingSize;
    }

    void Update()
    {
        if (groundCheck != null)
            Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!CanMove) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); return; }

        // ATTAQUE
        if (Input.GetKeyDown(attackKey))
        {
            if (CanAttack && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // MOUVEMENT
        float x = Input.GetAxisRaw("Horizontal");

        // ACCROUPISSEMENT
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

        float currentSpeed = isCrouching ? crouchSpeed : speed;
        rb.linearVelocity = new Vector2(x * currentSpeed, rb.linearVelocity.y);

        // SAUT
        if (Input.GetKeyDown(jumpKey) && Grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // DIRECTION DU REGARD
        if (x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // ANIMATION (Seulement si le renderer est visible, sinon ça veut dire qu'on attaque)
        if (anim != null && visualRenderer.enabled)
        {
            anim.SetFloat("Speed", Mathf.Abs(x));
            anim.SetBool("IsJumping", !Grounded);
        }
    }

    void Attack()
    {
        GestionEffetAttaqueManuel();
        if (attackPoint != null) Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
    }

    void GestionEffetAttaqueManuel()
    {
        if (attackEffectPrefab != null && attackSprites.Length > 0)
        {
            // 1. ON DESACTIVE L'IMAGE DU JOUEUR (IDLE/RUN)
            if (visualRenderer != null) visualRenderer.enabled = false;

            // 2. ON CRÉE L'EFFET D'ATTAQUE
            GameObject effet = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
            effet.transform.SetParent(this.transform);
            effet.transform.localScale = new Vector3(effectSizeMultiplier, effectSizeMultiplier, 1f);
            effet.transform.localPosition = effectOffset; 

            SpriteRenderer srEffet = effet.GetComponent<SpriteRenderer>();
            
            // On s'assure que l'effet est visible au-dessus de tout
            if (srEffet != null && visualRenderer != null) srEffet.sortingOrder = visualRenderer.sortingOrder + 1;

            StartCoroutine(AnimateEffectRoutine(srEffet, effet));
        }
    }

    IEnumerator AnimateEffectRoutine(SpriteRenderer sr, GameObject objToDestroy)
    {
        foreach (Sprite frame in attackSprites)
        {
            if(sr != null) sr.sprite = frame;
            yield return new WaitForSeconds(frameRate);
        }

        // 3. UNE FOIS FINI, ON RÉACTIVE L'IMAGE DU JOUEUR
        if (visualRenderer != null) visualRenderer.enabled = true;

        if(objToDestroy != null) Destroy(objToDestroy);
    }

    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag("Grounded")) Grounded = true; }
    private void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject.CompareTag("Grounded")) Grounded = false; }
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
        if (attackPoint != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
    }
}