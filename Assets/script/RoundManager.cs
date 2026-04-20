using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    [Header("External Scripts")]
    public HealthBarTest p1HealthScript; 
    public HealthBarTest p2HealthScript;
    public Timer timerScript;
    // public UltimateSystem p1UltimateScript;

    [Header("Score Settings")]
    public int p1Wins = 0;
    public int p2Wins = 0;
    public int roundsToWin = 2;

    private bool isRoundActive = true;

    void Start()
    {
        isRoundActive = true;
        Time.timeScale = 1f; 
        if (p1HealthScript != null) p1HealthScript.ResetHealth();
        if (p2HealthScript != null) p2HealthScript.ResetHealth();
        if (timerScript != null) timerScript.ResetTimer();
    }

    void Update()
    {
        if (!isRoundActive) return;

        if (p1HealthScript == null || p2HealthScript == null || timerScript == null)
        {
            Debug.LogWarning("RoundManager: Les barres de vie ou le timer manquent dans l'inspecteur !");
            return;
        }

        if (p1HealthScript.healthSlider.value <= 0f)
        {
            EndRound(2);
        }
        else if (p2HealthScript.healthSlider.value <= 0f)
        {
            EndRound(1); 
        }


        if (timerScript.timeLeft <= 0)
        {
            HandleTimeOut();
        }
    }

    void HandleTimeOut()
    {
        if (p1HealthScript.healthSlider.value > p2HealthScript.healthSlider.value) EndRound(1);
        else if (p2HealthScript.healthSlider.value > p1HealthScript.healthSlider.value) EndRound(2);
        else EndRound(0);
    }

    public void EndRound(int winner)
    {
        isRoundActive = false;
        timerScript.StopTimer(); 

        if (winner == 1) p1Wins++;
        if (winner == 2) p2Wins++;

        Debug.Log("Round Winner is Player " + winner);
        StartCoroutine(RoundEndSequence());
    }

    IEnumerator RoundEndSequence()
    {
        Time.timeScale = 0.4f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;

        if (p1Wins >= roundsToWin || p2Wins >= roundsToWin)
        {
            Debug.Log("FIN DU MATCH !");

            // reset rage fin match
            foreach (PlayerRage rage in FindObjectsOfType<PlayerRage>()) rage.ResetForMatch();
        }
        else
        {
            ResetNewRound();
        }
    }

    void ResetNewRound()
    {
        p1HealthScript.ResetHealth(); 
        p2HealthScript.ResetHealth();
        
        timerScript.ResetTimer();
        
        isRoundActive = true;
    }
}