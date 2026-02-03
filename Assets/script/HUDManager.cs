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

    public HealthBarTest p1HUD;
    public HealthBarTest p2HUD;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public HealthBarTest GetHUDForPlayer(int playerID)
    {
        return playerID == 1 ? p1HUD : p2HUD;
    }
}