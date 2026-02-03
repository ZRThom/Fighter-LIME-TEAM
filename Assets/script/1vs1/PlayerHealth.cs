using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int playerID = 1;
    private HealthBarTest hud;
    private int currentHealth;

    private PlayerState playerState;

    void Awake()
    {
        hud = HUDManager.instance.GetHUDForPlayer(playerID);
        if (hud != null) hud.SetHealth(1f);
        currentHealth = maxHealth;
        playerState = GetComponent<PlayerState>();
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
            hud.SetDamages(normalizedHealth); // absurd bug fix 1:30 (03/02/2026) (exponential damage)
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
}