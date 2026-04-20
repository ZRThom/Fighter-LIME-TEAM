using UnityEngine;
using System.Collections;

public class StoryGameManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerHealth p1;
    public PlayerHealth p3; // AI is P3 to avoid affecting 1v1 mode

    [Header("Spawn Points")]
    public Transform p1SpawnPos;
    public Transform p3SpawnPos;

    [Header("Story UI")]
    public StoryPanelManager panelManager;

    [Tooltip("Indicates which boss is faced (1, 2, 3, or 4)")]
    public int bossLevel = 1;

    private RoundManager roundManager;
    private int currentP1Wins;
    private int currentP2Wins;

    void Start()
    {
        roundManager = FindObjectOfType<RoundManager>();
        
        if (roundManager != null)
        {
            currentP1Wins = roundManager.p1Wins;
            currentP2Wins = roundManager.p2Wins;
        }
        else
        {
            Debug.LogWarning("Warning: No RoundManager found in the scene!");
        }

        ResetPositions();
    }

    void Update()
    {
        if (roundManager == null) return;
        
        if (roundManager.p1Wins > currentP1Wins || roundManager.p2Wins > currentP2Wins)
        {
            currentP1Wins = roundManager.p1Wins;
            currentP2Wins = roundManager.p2Wins;
            
            if (currentP1Wins < roundManager.roundsToWin && currentP2Wins < roundManager.roundsToWin)
            {
                StartCoroutine(WaitAndResetPositions());
            }
            else if (currentP1Wins >= roundManager.roundsToWin)
            {
                StartCoroutine(WaitAndShowPanel(true));
            }
            else if (currentP2Wins >= roundManager.roundsToWin)
            {
                StartCoroutine(WaitAndShowPanel(false));
            }
        }
    }

    IEnumerator WaitAndResetPositions()
    {
        yield return new WaitForSecondsRealtime(2f);
        ResetPositions();
    }

    IEnumerator WaitAndShowPanel(bool p1Won)
    {
        yield return new WaitForSecondsRealtime(2.5f);
        
        if (panelManager != null)
        {
            if (p1Won)
            {
                // Player 1 Victory: Show corresponding 'W' panel
                if (bossLevel == 1) panelManager.OpenW1();
                else if (bossLevel == 2) panelManager.OpenW2();
                else if (bossLevel == 3) panelManager.OpenW3();
                else if (bossLevel == 4) panelManager.OpenW4();
            }
            else
            {
                // Player 1 Defeat: Show corresponding 'L' panel
                if (bossLevel == 1) panelManager.OpenL1();
                else if (bossLevel == 2) panelManager.OpenL2();
                else if (bossLevel == 3) panelManager.OpenL3();
                else if (bossLevel == 4) panelManager.OpenL4();
            }
            
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("StoryGameManager Error: 'Panel Manager' is empty in the inspector!");
        }
    }

    public void ResetPositions()
    {
        if (p1 != null)
        {
            p1.gameObject.SetActive(true);
            p1.ResetHealth();

            PlayerRage p1Rage = p1.GetComponent<PlayerRage>();
            if (p1Rage != null) p1Rage.ResetForMatch();
        }
        if (p3 != null)
        {
            p3.gameObject.SetActive(true);
            p3.ResetHealth();

            PlayerRage p3Rage = p3.GetComponent<PlayerRage>();
            if (p3Rage != null) p3Rage.ResetForMatch();

            // Ensure Player 3 is set as AI
            PlayerState p3State = p3.GetComponent<PlayerState>();
            if (p3State != null) p3State.isAI = true;
        }

        if (p1 != null && p1SpawnPos != null) p1.transform.position = p1SpawnPos.position;
        if (p3 != null && p3SpawnPos != null) p3.transform.position = p3SpawnPos.position;

        if (p1 != null)
        {
            var rb1 = p1.GetComponent<Rigidbody2D>();
            if (rb1) rb1.linearVelocity = Vector2.zero;
        }
        if (p3 != null)
        {
            var rb3 = p3.GetComponent<Rigidbody2D>();
            if (rb3) rb3.linearVelocity = Vector2.zero;
        }
        Debug.Log("StoryGameManager: Positions reset.");
    }
}
