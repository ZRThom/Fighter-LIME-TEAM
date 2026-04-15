using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    public GameObject DialogueHolder, ContinueButton;
    public TextMeshProUGUI NameDisplay, TextDisplay;

    [Header("HealthBar HUD")]
    public HealthBarTest p1HUD;
    public HealthBarTest p2HUD;

    [Header("Rage HUD")]
    public RageBarUI p1RageHUD;
    public RageBarUI p2RageHUD;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public HealthBarTest GetHUDForPlayer(int playerID)
    {
        return playerID == 1 ? p1HUD : p2HUD;
    }

    public RageBarUI GetRageHUDForPlayer(int playerID)
    {
        return playerID == 1 ? p1RageHUD : p2RageHUD;
    }
}