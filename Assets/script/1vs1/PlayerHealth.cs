using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int playerID = 1;
    private HealthBarTest hud;
    public int currentHealth;

    private PlayerState playerState;
    private PlayerHitReact hitReact;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        hitReact = GetComponent<PlayerHitReact>();
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
        if (playerState != null) 
        {
            if (playerState.isShielding)
            {
                Debug.Log($"{gameObject.name} a bloqué les dégâts grâce au bouclier !");
                return;
            }

            if (playerState.isDead) return;
        }

        if (GetComponent<Manequin>() != null) return;

        int newHealth = Mathf.Max(currentHealth - damage, 0);
        bool willDie = newHealth <= 0;
        currentHealth = newHealth;

        if (hud != null)
        {
            float normalizedHealth = (float)currentHealth / maxHealth;
            hud.SetDamages(normalizedHealth);
        }

        if (willDie)
        {
            Die();
        }
        else
        {
            if (hitReact != null) hitReact.PlayHit();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} is ded");
        if (hitReact != null) hitReact.PlayDeath();
    }

    public int GetCurrentHealth() => currentHealth;
    //void OnDrawGizmosSelected()
    //{
    //    UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"Health: {currentHealth}/{maxHealth}");
    //}

    public void ResetHealth()
    {
        gameObject.SetActive(true);
        currentHealth = maxHealth;
        if (playerState != null)
        {
            playerState.isDead = false;
            playerState.isHit = false;
            playerState.animationLocked = false;
        }

        if (hitReact != null) hitReact.ResetReactionState();

        if (hud != null) hud.SetHealth(1f);
    }
}
