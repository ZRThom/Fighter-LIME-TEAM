using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int playerID = 1;
    private HealthBarTest hud;
    public int currentHealth;

    private PlayerState playerState;

    void Awake()
    {
        // On ne garde ici que ce qui est interne au personnage
        currentHealth = maxHealth;
        playerState = GetComponent<PlayerState>();
    }

    void Start()
    {
        // On attend le Start pour chercher le HUD, 
        // comme ça on est sûr que HUDManager.instance existe !
        if (HUDManager.instance != null)
        {
            hud = HUDManager.instance.GetHUDForPlayer(playerID);
            if (hud != null) hud.SetHealth(1f);
        }
        else
        {
            Debug.LogWarning("HUDManager instance non trouvée au Start !");
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

        if (currentHealth == 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth() => currentHealth;
}

