using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Vérifie si le bouclier est actif
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null && pm.IsShieldActive())
        {
            Debug.Log($"{gameObject.name} a bloqué les dégâts grâce au bouclier !");
            return; // attaque bloquée
        }

        // Sinon on applique les dégâts
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} prend {damage} dégâts. Santé actuelle : {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        gameObject.SetActive(false);
    }
}
