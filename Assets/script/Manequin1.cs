using UnityEngine;
using System.Collections;

public class Manequin : MonoBehaviour
{
    [Header("Configuration Vie")]
    public int health = 100;

    [Header("Configuration Visuelle")]
    public SpriteRenderer targetRenderer;
    public Sprite normalSprite;

    [Header("Animation de HIT")]
    public Sprite[] hitFrames;      
    public float hitFrameDuration = 0.1f;

    [Header("Texte Flottant")]
    public GameObject floatingTextPrefab; 
    // Ajuste Y ici (par ex: 1.5 ou 2.0) pour que le texte apparaisse au-dessus de la tête
    public Vector3 textOffset = new Vector3(0, 1.5f, 0); 

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (targetRenderer != null && normalSprite != null)
            targetRenderer.sprite = normalSprite;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0) health = 0;

        // Lancer l'animation de coup
        if (targetRenderer != null && hitFrames.Length > 0)
        {
            StopAllCoroutines(); 
            StartCoroutine(PlayHitAnimation());
        }

        // Faire apparaître le texte de dégâts
        ShowFloatingText(damageAmount);
    }

    IEnumerator PlayHitAnimation()
    {
        foreach (Sprite s in hitFrames)
        {
            targetRenderer.sprite = s;
            yield return new WaitForSeconds(hitFrameDuration);
        }

        if(normalSprite != null)
            targetRenderer.sprite = normalSprite;
    }

    void ShowFloatingText(int damage)
    {
        if (floatingTextPrefab != null)
        {
            // Calcul de la position avec l'offset (hauteur)
            Vector3 finalPosition = transform.position + textOffset;
            
            GameObject textObj = Instantiate(floatingTextPrefab, finalPosition, Quaternion.identity);

            // Vérification et assignation des dégâts
            // Assure-toi que ton script "FloatingText" a bien une méthode "SetDamage(int)"
            FloatingText textScript = textObj.GetComponent<FloatingText>();
            if (textScript != null)
            {
                textScript.SetDamage(damage);
            }
        }
    }
}