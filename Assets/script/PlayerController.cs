using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    // --- PARTIE STATS ---
    [Header("Stats du Joueur")]
    [SerializeField] private string playerName;
    [SerializeField] private int health = 100;
    [SerializeField] private int score = 0;
    
    [Header("Équipement")]
    [SerializeField] private string weapon;

    // --- PARTIE MOUVEMENT ---
    [Header("Paramètres Mouvement")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    // Variables techniques
    private CharacterController controller;
    private Vector3 velocity;

    void Start() 
    {
        controller = GetComponent<CharacterController>();
    }

    void Update() 
    { 
        // 1. Gestion du sol (reset gravité)
        if (controller.isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f; 
        }

        // 2. Déplacement (Z/Q/S/D)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 3. Saut
        if (Input.GetButtonDown("Jump") && controller.isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 4. Gravité
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); 
    }
}