using UnityEngine;
using System.Collections;
using TMPro;

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

    [Header("UI & Effects (Story)")]
    public StoryCountDownUI countDownUI;
    
    [Header("KO Effect")]
    public GameObject koRoot;
    public CanvasGroup koCanvasGroup;
    public RectTransform koRect;
    public float slowMoScale = 0.4f;
    public float slowMoDuration = 2f;
    public float koDuration = 3f;
    public float koFadeDuration = 1f;
    public float koStartScale = 10f;
    public float koEndScale = 2f;

    [Header("Win panel")]
    public GameObject winPanel;
    public TMP_Text winText;
    public float winPanelDurationInRound = 1.5f;

    [Header("Round win icon")]
    public GameObject[] p1RoundsWinIcons;
    public GameObject[] p2RoundsWinIcons;

    private bool isRoundActive = true;
    private bool isEndingRound = false;

    void Start()
    {
        Time.timeScale = 1f; 
        isRoundActive = false;
        isEndingRound = false;

        if (p1HealthScript != null) p1HealthScript.ResetHealth();
        if (p2HealthScript != null) p2HealthScript.ResetHealth();
        if (timerScript != null) timerScript.StopTimer();

        if (koRoot != null) koRoot.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        UpdateRoundWinIcons();

        StartCoroutine(StartRoundCountdown());
    }

    void Update()
    {
        if (!isRoundActive || isEndingRound) return;

        if (p1HealthScript == null || p2HealthScript == null || timerScript == null)
        {
            Debug.LogWarning("RoundManager: Les barres de vie ou le timer manquent dans l'inspecteur !");
            return;
        }

        if (p1HealthScript.healthSlider.value <= 0f)
        {
            EndRound(2);
            return;
        }
        
        if (p2HealthScript.healthSlider.value <= 0f)
        {
            EndRound(1);
            return;
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
        if (!isRoundActive || isEndingRound) return;
        isRoundActive = false;
        isEndingRound = true;
        timerScript.StopTimer(); 

        SetPlayersControl(false);

        Debug.Log("Round Winner is Player " + winner);
        StartCoroutine(RoundEndSequence(winner));
    }

    IEnumerator RoundEndSequence(int winner)
    {
        Time.timeScale = slowMoScale;
        yield return new WaitForSecondsRealtime(slowMoDuration);
        Time.timeScale = 1f;

        yield return StartCoroutine(PlayKoEffect());
        ShowWinnerPanel(winner);
        
        yield return new WaitForSecondsRealtime(winPanelDurationInRound);
        
        if (winPanel != null) winPanel.SetActive(false);

        if (winner == 1) p1Wins++;
        if (winner == 2) p2Wins++;

        UpdateRoundWinIcons();

        if (p1Wins >= roundsToWin || p2Wins >= roundsToWin)
        {
            Debug.Log("FIN DU MATCH !");

            foreach (PlayerRage rage in FindObjectsOfType<PlayerRage>()) rage.ResetForMatch();
            isEndingRound = false;
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
            isEndingRound = false;
            ResetNewRound();
        }
    }

    private void UpdateRoundWinIcons()
    {
        if (p1RoundsWinIcons != null)
        {
            for (int i = 0; i < p1RoundsWinIcons.Length; i++)
            {
                if (p1RoundsWinIcons[i] != null) p1RoundsWinIcons[i].SetActive(i < p1Wins);
            }
        }
        if (p2RoundsWinIcons != null)
        {
            for (int i = 0; i < p2RoundsWinIcons.Length; i++)
            {
                if (p2RoundsWinIcons[i] != null) p2RoundsWinIcons[i].SetActive(i < p2Wins);
            }
        }
    }

    void ResetNewRound()
    {
        p1HealthScript.ResetHealth(); 
        p2HealthScript.ResetHealth();
        
        StartCoroutine(StartRoundCountdown());
    }

    private IEnumerator StartRoundCountdown()
    {
        isRoundActive = false;

        SetPlayersControl(false);
        if (timerScript != null) timerScript.StopTimer();
        
        if (countDownUI != null)
        {
            countDownUI.PlayCountdown();
            yield return new WaitForSecondsRealtime(countDownUI.TotalDuration);
        }
        
        SetPlayersControl(true);
        if (timerScript != null) timerScript.ResetTimer();

        isRoundActive = true;
    }

    private IEnumerator PlayKoEffect()
    {
        if (koRoot == null || koCanvasGroup == null || koRect == null)
        {
            yield return new WaitForSecondsRealtime(koDuration);
            yield break;
        }

        koRoot.SetActive(true);
        koCanvasGroup.alpha = 0f;
        koRect.localScale = Vector3.one * koStartScale;

        float t = 0f;

        while (t < koDuration)
        {
            t += Time.unscaledDeltaTime;

            float fadeT = Mathf.Clamp01(t / koFadeDuration);
            float scaleT = Mathf.Clamp01(t / koFadeDuration);

            koCanvasGroup.alpha = fadeT;

            float scale = Mathf.Lerp(koStartScale, koEndScale, scaleT);
            koRect.localScale = Vector3.one * scale;

            yield return null;
        }

        koCanvasGroup.alpha = 1f;
        koRect.localScale = Vector3.one * koEndScale;

        yield return new WaitForSecondsRealtime(0.1f);

        koRoot.SetActive(false);
    }

    private void ShowWinnerPanel(int winner)
    {
        if (winPanel != null) winPanel.SetActive(true);

        if (winText != null)
        {
            if (winner == 0) winText.text = "ÉGALITÉ";
            else if (winner == 1) winText.text = "JOUEUR 1 REMPORTE CE ROUND !";
            else winText.text = "NPC BOSS WIN";
        }
    }

    private void SetPlayersControl(bool state)
    {
        PlayerInputHandler[] handlers = FindObjectsOfType<PlayerInputHandler>();
        foreach (var h in handlers)
        {
            h.SetInputsEnabled(state);
            if (!state) h.ClearInputs();
        }

        PlayerController2D[] oldControllers = FindObjectsOfType<PlayerController2D>();
        foreach(var c in oldControllers)
        {
            c.CanMove = state;
            c.CanAttack = state;
        }
    }
}