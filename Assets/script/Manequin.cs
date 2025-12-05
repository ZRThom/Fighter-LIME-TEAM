using UnityEngine;

public class Manequin : MonoBehaviour
{
    [Header("Configuration")]
    public int health = 100;
    public GameObject floatingTextPrefab; // Glisse ton Prefab "DamagePopup" ici
    public Vector3 textOffset = new Vector3(0, 2f, 0); // Hauteur d'apparition du texte

    // Variables privées (gérées par le script)
    private Animator _animator;

    void Start()
    {
        // Le script cherche tout seul le composant Animator sur l'objet
        _animator = GetComponent<Animator>();
    }

    // Fonction appelée quand on frappe le mannequin
    public void TakeDamage(int damageAmount)
    {
        // 1. Gérer la vie
        health -= damageAmount;
        if (health < 0) health = 0;

        // 2. Gérer l'Animation
        if (_animator != null)
        {
            // Lance l'animation associée au trigger "Hit"
            _animator.SetTrigger("Hit");
        }

        // 3. Gérer le Texte Flottant
        ShowFloatingText(damageAmount);
    }

    void ShowFloatingText(int damage)
    {
        if (floatingTextPrefab != null)
        {
            // Crée le texte à la position du mannequin + le décalage (offset)
            GameObject textObj = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);

            // Récupère le script FloatingText que tu m'as montré et envoie le chiffre
            FloatingText textScript = textObj.GetComponent<FloatingText>();
            
            if (textScript != null)
            {
                textScript.SetDamage(damage);
            }
        }
    }
}