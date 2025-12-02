using UnityEngine;

public class PlayerController2D : MonoBehaviour 
{
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

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;
    private bool isCrouching;
    
    // NOUVEAU : On compte les sauts
    private int jumpCount = 0; 
    private int maxJumps = 1; // Limite stricte à 1

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

        // Si on touche le sol, on remet le compteur à 0
        if (isGrounded && rb.linearVelocity.y <= 0.1f) // Petite sécurité sur la vitesse Y
        {
            jumpCount = 0;
        }

        // 2. Mouvement & Accroupissement
        float x = Input.GetAxisRaw("Horizontal");

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

        // 3. Saut (LOGIQUE MODIFIÉE)
        // On vérifie si on n'a pas dépassé le nombre max de sauts
        if (Input.GetKeyDown(jumpKey) && jumpCount < maxJumps && !isCrouching) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++; // On ajoute 1 au compteur
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