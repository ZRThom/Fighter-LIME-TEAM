using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // On garde une référence vers le PlayerState pour savoir si on bouclier est actif
    private PlayerState playerState;

    void Awake()
    {
        currentHealth = maxHealth;
        
        // Dans la nouvelle architecture, c'est PlayerState qui sait tout
        playerState = GetComponent<PlayerState>();
    }

    public void TakeDamage(int damage)
    {
        // 1. Vérification du bouclier via le nouveau script PlayerState
        if (playerState != null && playerState.isShielding)
        {
            Debug.Log($"{gameObject.name} a bloqué les dégâts grâce au bouclier !");
            return;
        }

        // 2. Application des dégâts
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log($"{gameObject.name} prend {damage} dégâts. Santé actuelle : {currentHealth}");

        if (currentHealth == 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        gameObject.SetActive(false);
        // Plus tard, tu peux déclencher un événement pour GameManager
        //OnDeath?.Invoke();
    }

    public int GetCurrentHealth() => currentHealth;
    void OnDrawGizmosSelected()
    {
        // Affiche la santé actuelle au-dessus du joueur dans l'éditeur
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"Health: {currentHealth}/{maxHealth}");
    }
}