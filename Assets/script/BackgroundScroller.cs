using UnityEngine;
using UnityEngine;
using UnityEngine.UI; // Ajouté : Nécessaire pour le composant Image

public class BackgroundScroller : MonoBehaviour
{
    [Header("---- VISUAL ----")]
    public Image visualRenderer; 
    
    [Tooltip("Images d'animation du background (laisser vide si pas d'animation)")]
    public Sprite[] backgroundSprites;
    [Tooltip("Temps entre chaque image pour l'animation (si backgroundSprites est utilisé)")]
    public float animSpeed = 0.1f;

    [Header("---- Scaling ----")]
    [Tooltip("Vitesse de défilement horizontal (positive pour aller vers la gauche)")]
    [SerializeField] private float scrollSpeed = 1f;

    [Tooltip("La position de fin où le background doit revenir à sa position de départ (utile pour les backgrounds répétitifs)")]
    [SerializeField] private float resetPositionX = -20f; 

    [Tooltip("La position de départ pour le reset")]
    [SerializeField] private float startPositionX = 20f;

    private float animTimer;
    private int currentFrame;

    void Start()
    {
        if (visualRenderer == null)
        {
            visualRenderer = GetComponent<Image>();
            if (visualRenderer == null)
            {
                // error log
                Debug.LogError("BackgroundScroller nécessite un composant Image (UI) sur le même GameObject.");
                enabled = false; 
                return;
            }
        }

        // start animation
        if (backgroundSprites != null && backgroundSprites.Length > 0)
        {
            // change
            visualRenderer.sprite = backgroundSprites[0]; 
        }
        else
        {
            // safety
            visualRenderer.sprite = null;
        }
    }

    void Update()
    {
        HandleScrolling();

        HandleManualAnimation();
    }

    void HandleScrolling()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x <= resetPositionX)
        {
            transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
        }
    }

    void HandleManualAnimation()
    {
        if (visualRenderer == null || backgroundSprites == null || backgroundSprites.Length <= 1) return;

        animTimer += Time.deltaTime;
        
        if (animTimer >= animSpeed)
        {
            animTimer = 0;
            currentFrame++;
            
            // animation loop
            if (currentFrame >= backgroundSprites.Length) 
            {
                currentFrame = 0;
            }
            
            visualRenderer.sprite = backgroundSprites[currentFrame];
        }
    }
}