using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;
    
    [Header("États")]
    public bool CanMove = true;
    public bool CanAttack = true;

    [Header("Mouvement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Contrôles")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0; // Clic gauche par défaut

    [Header("Combat")]
    [SerializeField] private Transform attackPoint; // Point central de l'attaque
    [SerializeField] private float attackRange = 0.5f; // Rayon de l'attaque
    [SerializeField] private LayerMask enemyLayers; // Quels layers sont des ennemis ?
    [SerializeField] private float attackRate = 2f; // Attaques par seconde
    [SerializeField] private int attackDamage = 10;
    private float nextAttackTime = 0f;

    [Header("Accroupissement")]
    [SerializeField] private Vector2 standingSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 standingOffset = Vector2.zero;
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.25f);

    [Header("Détection du Sol")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Variables privées
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;
    
    [SerializeField] private bool isCrouching;
    [SerializeField] bool Grounded;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>(); 

        if (anim == null) Debug.LogError("ATTENTION : Pas d'Animator trouvé sur le joueur !");
        if (attackPoint == null) Debug.LogError("ATTENTION : Le champ 'Attack Point' est vide dans l'inspecteur !");

        col.size = standingSize;
        col.offset = standingOffset;
    }

    void Update()
    {
        // 1. Vérifier le sol
        if (groundCheck != null)
        {
            Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // --- BLOCAGE DES MOUVEMENTS ---
        if (!CanMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return; 
        }

        // 2. Gestion de l'Attaque (Debug version)
        // On sépare la détection de la touche du reste pour comprendre ce qui échoue
        if (Input.GetKeyDown(attackKey))
        {
            Debug.Log("--- Tentative d'attaque ---");
            
            if (!CanAttack)
            {
                Debug.Log("Bloqué : CanAttack est faux.");
            }
            else if (Time.time < nextAttackTime)
            {
                Debug.Log("Bloqué : Cooldown en cours (trop rapide).");
            }
            else
            {
                // Tout est bon, on lance l'attaque
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // 3. Mouvement
        float x = Input.GetAxisRaw("Horizontal");

        // Accroupissement
        if (Grounded && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            isCrouching = true;
            col.size = crouchSize;
            col.offset = crouchOffset;
        }
        else
        {
            isCrouching = false;
            col.size = standingSize;
            col.offset = standingOffset;
        }

        float currentSpeed = isCrouching ? crouchSpeed : speed;
        rb.linearVelocity = new Vector2(x * currentSpeed, rb.linearVelocity.y);

        // 4. Saut
        if (Input.GetKeyDown(jumpKey) && Grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 5. Flip
        if (x > 0) 
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (x < 0) 
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        
        // Gestion Animation
        if(anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(x));
            anim.SetBool("IsJumping", !Grounded);
        }
    }

    // --- FONCTION D'ATTAQUE ---
    void Attack()
    {
        Debug.Log(">> Lancement de la fonction Attack()");

        // Sécurité : si pas de point d'attaque, on arrête pour éviter le crash
        if (attackPoint == null)
        {
            Debug.LogError("ERREUR : Assigne un Transform à 'Attack Point' dans l'inspecteur !");
            return;
        }

        // 1. Jouer l'animation
        if(anim != null)
        {
            anim.SetTrigger("Attack"); // Assure-toi que le paramètre s'appelle bien "Attack" (sensible à la casse)
            Debug.Log(">> Trigger 'Attack' envoyé à l'Animator.");
        }

        // VISUEL TEMPORAIRE : On dessine une croix rouge là où ça tape
        // (Visible dans l'onglet 'Scene' quand le jeu tourne, pas dans 'Game')
        Debug.DrawLine(attackPoint.position, attackPoint.position + Vector3.right * attackRange, Color.red, 1f);
        Debug.DrawLine(attackPoint.position, attackPoint.position + Vector3.up * attackRange, Color.red, 1f);

        // 2. Détecter les ennemis
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. Appliquer les dégâts
        if (hitEnemies.Length > 0)
        {
            foreach(Collider2D enemy in hitEnemies)
            {
                Debug.Log(">> TOUCHÉ : " + enemy.name);
                // enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
            }
        }
        else
        {
            Debug.Log(">> Coup dans le vide (aucun collider sur le Layer Ennemi).");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grounded"))
        {
            Grounded = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grounded"))
        {
            Grounded = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}