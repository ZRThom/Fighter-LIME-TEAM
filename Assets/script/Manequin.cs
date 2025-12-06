using UnityEngine;
using System.Collections;

public class Manequin : MonoBehaviour
{
    [Header("Configuration Vie")]
    public int health = 100;

    [Header("Configuration Visuelle")]
    // Si tu veux, tu peux glisser l'objet enfant qui contient l'image ici.
    // Sinon, le script le cherchera tout seul.
    public SpriteRenderer targetRenderer; 
    
    public Sprite normalSprite;
    public Sprite hitSprite;
    public float hitDuration = 0.2f;

    [Header("Texte Flottant")]
    public GameObject floatingTextPrefab;
    public Vector3 textOffset = new Vector3(0, 2f, 0);

    void Start()
    {
        // Si tu n'as pas rempli la case manuellement, on cherche dans les enfants
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // On vérifie si on a bien trouvé le composant
        if (targetRenderer != null && normalSprite != null)
        {
            targetRenderer.sprite = normalSprite;
        }
        else
        {
            Debug.LogError("Attention : Aucun SpriteRenderer trouvé dans le Mannequin ou ses enfants !");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0) health = 0;

        // Gestion du Sprite (vérifie qu'on a bien trouvé le renderer)
        if (targetRenderer != null && hitSprite != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashSprite());
        }

        ShowFloatingText(damageAmount);
    }

    IEnumerator FlashSprite()
    {
        targetRenderer.sprite = hitSprite;
        yield return new WaitForSeconds(hitDuration);
        targetRenderer.sprite = normalSprite;
    }

    void ShowFloatingText(int damage)
    {
        if (floatingTextPrefab != null)
        {
            // Note: On ne met pas le texte en enfant, sinon il bougerait bizarrement avec le mannequin
            GameObject textObj = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);
            
            FloatingText textScript = textObj.GetComponent<FloatingText>();
            if (textScript != null)
            {
                textScript.SetDamage(damage);
            }
        }
    }
}