using UnityEngine;
using System.Collections;

public class StoryGameManager : MonoBehaviour
{
    [Header("Players")]
    public PlayerHealth p1;
    public PlayerHealth p3;

    [Header("Spawn Points")]
    public Transform p1SpawnPos;
    public Transform p3SpawnPos;

    [Header("Story UI")]
    public StoryPanelManager panelManager;

    [Tooltip("Which boss is being fought (1, 2, 3, or 4)")]
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
            Debug.LogWarning("<color=orange>WARNING:</color> No RoundManager found in the scene!");
        }

        ResetPositions();
    }

    void Update()
    {
        if (roundManager == null) return;
        
        // Check if a round has just been won
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
                StartCoroutine(WaitAndShowPanel(true)); // Player 1 Wins Match
            }
            else if (currentP2Wins >= roundManager.roundsToWin)
            {
                StartCoroutine(WaitAndShowPanel(false)); // Boss Wins Match
            }
        }
    }

    IEnumerator WaitAndResetPositions()
    {
        yield return null; 
        ResetPositions();
    }

    IEnumerator WaitAndShowPanel(bool p1Won)
    {
        yield return new WaitForSecondsRealtime(2.5f);
        
        if (panelManager != null)
        {
            if (p1Won)
            {
                // SAVE PROGRESSION: This is what unlocks characters in Shop and Selection
                int previousProgression = PlayerPrefs.GetInt("DernierStageFini", 0);
                
                if (bossLevel > previousProgression)
                {
                    PlayerPrefs.SetInt("DernierStageFini", bossLevel);
                    PlayerPrefs.Save();
                    Debug.Log("<color=green>SAVE SUCCESSFUL:</color> Boss " + bossLevel + " defeated! Progress updated.");
                }

                // Open the correct Win Panel
                if (bossLevel == 1) panelManager.OpenW1();
                else if (bossLevel == 2) panelManager.OpenW2();
                else if (bossLevel == 3) panelManager.OpenW3();
                else if (bossLevel == 4) panelManager.OpenW4();
            }
            else
            {
                Debug.Log("<color=red>GAME OVER:</color> Player lost against Boss " + bossLevel);
                
                // Open the correct Lose Panel
                if (bossLevel == 1) panelManager.OpenL1();
                else if (bossLevel == 2) panelManager.OpenL2();
                else if (bossLevel == 3) panelManager.OpenL3();
                else if (bossLevel == 4) panelManager.OpenL4();
            }
            
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Debug.LogError("<color=red>ERROR:</color> 'StoryPanelManager' is missing in the Inspector!");
        }
    }

    public void ResetPositions()
    {
        // Player 1 Reset
        if (p1 != null)
        {
            p1.gameObject.SetActive(true);
            p1.ResetHealth();
            PlayerRage p1Rage = p1.GetComponent<PlayerRage>();
            if (p1Rage != null) p1Rage.ResetForMatch();
            if (p1SpawnPos != null) p1.transform.position = p1SpawnPos.position;

            var rb1 = p1.GetComponent<Rigidbody2D>();
            if (rb1) rb1.linearVelocity = Vector2.zero;
        }

        // Boss (P3) Reset
        if (p3 != null)
        {
            p3.gameObject.SetActive(true);
            p3.ResetHealth();
            PlayerRage p3Rage = p3.GetComponent<PlayerRage>();
            if (p3Rage != null) p3Rage.ResetForMatch();

            PlayerState p3State = p3.GetComponent<PlayerState>();
            if (p3State != null) p3State.isAI = true; // Force AI mode for Story

            if (p3SpawnPos != null) p3.transform.position = p3SpawnPos.position;

            var rb3 = p3.GetComponent<Rigidbody2D>();
            if (rb3) rb3.linearVelocity = Vector2.zero;
        }

        Debug.Log("<color=white>STORY MODE:</color> Positions and stats reset.");
    }
}