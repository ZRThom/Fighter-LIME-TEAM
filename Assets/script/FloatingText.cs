using UnityEngine;
using TMPro; 

public class FloatingText : MonoBehaviour
{
    [Header("Réglages")]
    public float destroyTime = 1f;        
    public Vector3 moveSpeed = new Vector3(0, 2f, 0); 

    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Start()
    {
        // --- SPÉCIAL 2D ---
        // On récupère le Renderer du texte pour forcer l'affichage au premier plan
        MeshRenderer render = GetComponent<MeshRenderer>();
        if (render != null)
        {
            // Met le texte sur un ordre très élevé pour qu'il soit devant les sprites
            render.sortingOrder = 50; 
        }
        // ------------------

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // En 2D, on bouge aussi sur les axes X/Y, donc ça marche pareil
        transform.position += moveSpeed * Time.deltaTime;
    }

    public void SetDamage(int damageAmount)
    {
        if (textComponent != null)
        {
            textComponent.text = damageAmount.ToString();
        }
    }
}