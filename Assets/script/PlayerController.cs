using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;
    public bool CanMove = true;
    public bool CanAttack = true;

    [Header("Mouvement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Contrôles")]
    public KeyCode jumpKey = KeyCode.Space;

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
    private bool isGrounded;
    private bool isCrouching;
    private int jumpCount = 0;
    private int maxJumps = 1;


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
        col.size = standingSize;
        col.offset = standingOffset;
    }

    void Update()
    {
        // 1. Vérifier le sol
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpCount = 0;
        }

        // --- BLOCAGE DES MOUVEMENTS ---
        if (!CanMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return; // On arrête tout ici si le joueur ne peut pas bouger
        }
        // ------------------------------

        // 2. Mouvement
        float x = Input.GetAxisRaw("Horizontal");

        // Accroupissement
        if (isGrounded && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
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

        // 3. Saut
        if (Input.GetKeyDown(jumpKey) && jumpCount < maxJumps && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }

        // 4. Flip
        if (x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}