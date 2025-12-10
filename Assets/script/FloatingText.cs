using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private TMP_Text textComponent;
    private float timer;
    private Color startColor;

    [Header("Réglages")]
    public float speed = 2f;
    public float duration = 1f;

    void Start()
    {
        // 1. On cherche le composant Texte
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        // 2. On configure la couleur de départ
        if (textComponent != null)
        {
            startColor = textComponent.color;
        }
        else
        {
             Debug.LogError("ERREUR : Pas de TMP_Text trouvé sur le FloatingText !");
        }

        // 3. CORRECTION DE L'ERREUR : On utilise le MeshRenderer pour le Sorting Order
        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        if (mesh != null)
        {
            mesh.sortingOrder = 500; // Force l'affichage devant tout le reste
            mesh.sortingLayerName = "Default";
        }

        // 4. Petit décalage aléatoire
        transform.localPosition += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
    }

    public void SetDamage(int damageAmount)
    {
        // Sécurité : On cherche le texte si on ne l'a pas encore
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        // On applique le chiffre
        if(textComponent != null)
        {
            textComponent.text = damageAmount.ToString();
        }
    }

    void Update()
    {
        // Monter vers le haut
        transform.position += Vector3.up * speed * Time.deltaTime;
        
        // Gestion de la disparition
        timer += Time.deltaTime;
        if (textComponent != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            textComponent.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}