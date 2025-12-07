using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // --- PARAMÈTRES VISUELS ---
    [Header("---- VISUELS ----")]
    public SpriteRenderer visualRenderer;
    [Tooltip("Images d'animation du background (laisser vide si pas d'animation)")]
    public Sprite[] backgroundSprites;
    [Tooltip("Temps entre chaque image pour l'animation (si backgroundSprites est utilisé)")]
    public float animSpeed = 0.1f;

    // --- PARAMÈTRES DE DÉFILEMENT ---
    [Header("---- DÉFILEMENT ----")]
    [Tooltip("Vitesse de défilement horizontal (positive pour aller vers la gauche)")]
    [SerializeField] private float scrollSpeed = 1f;

    [Tooltip("La position de fin où le background doit revenir à sa position de départ (utile pour les backgrounds répétitifs)")]
    [SerializeField] private float resetPositionX = -20f; 

    [Tooltip("La position de départ pour le reset")]
    [SerializeField] private float startPositionX = 20f;

    // --- VARIABLES PRIVÉES D'ANIMATION ---
    private float animTimer;
    private int currentFrame;

    // -------------------------------------------------------------------------
    // RAPPEL : Le Rigidbody2D, le Collider, la Santé, les Inputs, et toutes les
    // méthodes de mouvement/combat (saut, attaque, course, accroupissement) 
    // ont été **supprimés** car ils ne sont pas pertinents pour un arrière-plan.
    // -------------------------------------------------------------------------

    void Start()
    {
        // Assurez-vous d'avoir un SpriteRenderer
        if (visualRenderer == null)
        {
            visualRenderer = GetComponent<SpriteRenderer>();
            if (visualRenderer == null)
            {
                Debug.LogError("BackgroundScroller nécessite un SpriteRenderer sur le même GameObject.");
                enabled = false; // Désactive le script si pas de Renderer
                return;
            }
        }

        // Commence l'animation au premier sprite (si disponible)
        if (backgroundSprites != null && backgroundSprites.Length > 0)
        {
            visualRenderer.sprite = backgroundSprites[0];
        }
    }

    void Update()
    {
        // 1. --- LOGIQUE DE DÉFILEMENT ---
        HandleScrolling();

        // 2. --- GESTION DE L'ANIMATION DU BACKGROUND (si sprites multiples) ---
        HandleManualAnimation();
    }

    /// <summary>
    /// Déplace le background et le réinitialise quand il sort de l'écran.
    /// </summary>
    void HandleScrolling()
    {
        // Déplace le background vers la gauche (si scrollSpeed est positif)
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Vérifie si le background a atteint le point de réinitialisation
        if (transform.position.x <= resetPositionX)
        {
            // Réinitialise la position pour simuler une boucle infinie
            transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Gère le changement de sprites pour l'animation en boucle du background.
    /// </summary>
    void HandleManualAnimation()
    {
        if (visualRenderer == null || backgroundSprites == null || backgroundSprites.Length <= 1) return;

        animTimer += Time.deltaTime;
        
        if (animTimer >= animSpeed)
        {
            animTimer = 0;
            currentFrame++;
            
            // Boucle l'animation
            if (currentFrame >= backgroundSprites.Length) 
            {
                currentFrame = 0;
            } 
            
            // Applique le sprite
            visualRenderer.sprite = backgroundSprites[currentFrame];
        }
    }
}