using UnityEngine;

[RequireComponent(typeof(PlayerConfig))]
public class PlayerRage : MonoBehaviour
{
    private PlayerConfig config;
    private RageBarUI hud;

    public float CurrentRage { get; private set; }
    public bool IsFull => config != null && CurrentRage >= config.maxRage;
    void Start()
    {
        config = GetComponent<PlayerConfig>();
        RefreshHUD();
    }

    void RefreshHUD()
    {
        if (config == null)
        {
            return;
        }

        if (hud == null && HUDManager.instance != null)
        {
            hud = HUDManager.instance.GetRageHUDForPlayer(config.playerNumber);
        }

        if (hud != null)
        {
            hud.SetValue(CurrentRage, config.maxRage);
        }
    }

    public void AddRage(float amount)
    {
        if (config == null) config = GetComponent<PlayerConfig>();

        CurrentRage = Mathf.Clamp(CurrentRage + amount, 0f, config.maxRage);
        RefreshHUD();
    }

    public bool ConsumeFullRage()
    {
        if (!IsFull)
        {
            return false;
        }
        CurrentRage = 0f;
        RefreshHUD();
        return true;
    }

    public void ResetForMatch()
    {
        CurrentRage = 0f;
        RefreshHUD();
    }
}
