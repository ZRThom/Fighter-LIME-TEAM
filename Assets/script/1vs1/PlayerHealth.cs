using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int playerID = 1;
    private HealthBarTest hud;
    public int currentHealth;

    private PlayerState playerState;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        currentHealth = maxHealth;

        if (HUDManager.instance != null)
        {
            hud = HUDManager.instance.GetHUDForPlayer(playerID);
            if (hud != null) hud.SetHealth(1f);
            else Debug.LogWarning($"HUD not found for player {playerID}");
        }
        else
        {
            Debug.LogWarning("HUDManager null");
        }
    }

    public void TakeDamage(int damage)
    {
        if (playerState != null && playerState.isShielding)
        {
            Debug.Log($"{gameObject.name} a bloqué les dégâts grâce au bouclier !");
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        if (hud != null)
        {
            float normalizedHealth = (float)currentHealth / maxHealth;
            hud.SetDamages(normalizedHealth);
        }
        Debug.Log($"{gameObject.name} prend {damage} dégâts. Santé actuelle : {currentHealth}");

        if (currentHealth == 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth() => currentHealth;
    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"Health: {currentHealth}/{maxHealth}");
    }

    public void ResetHealth()
    {
        gameObject.SetActive(true);
        currentHealth = maxHealth;
        if (hud != null) hud.SetHealth(1f);
    }
}