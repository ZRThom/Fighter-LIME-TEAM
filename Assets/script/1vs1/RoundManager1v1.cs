using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class RoundManager1v1 : MonoBehaviour
{
    [Header("External Scripts")]
    [SerializeField] private HealthBarTest p1HealthScript; 
    [SerializeField] private HealthBarTest p2HealthScript;
    [SerializeField] private Timer timerScript;
    // public UltimateSystem p1UltimateScript;

    [Header("Score Settings")]
    [SerializeField] private int p1Wins = 0;
    [SerializeField] private int p2Wins = 0;
    [SerializeField] private int roundsToWin = 2;

    [Header("KO")]
    [SerializeField] private GameObject koRoot;
    [SerializeField] private CanvasGroup koCanvasGroup;
    [SerializeField] private RectTransform  koRect;
    [SerializeField] private float slowMoScale = 0.4f;
    [SerializeField] private float slowMoDuration = 2f;
    [SerializeField] private float koDuration = 3f;
    [SerializeField] private float koFadeDuration = 1f;
    [SerializeField] private float koStartScale = 10f;
    [SerializeField] private float koEndScale = 2f;

    [Header("Win panel")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private float winPanelDurationInRound = 1.5f;

    [Header("Round win icon")]
    [SerializeField] private GameObject[] p1RoundsWinIcons;
    [SerializeField] private GameObject[] p2RoundsWinIcons;

    [Header("End panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endWinText;
    [SerializeField] private float endMenuDelay = 2f;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMENU";
    [SerializeField] private string characterSelectSceneName = "MainMENU";

    private bool isRoundActive = true;
    private bool isEndingRound = false;

    void Start()
    {
        Time.timeScale = 1f; 
        isRoundActive = true;
        isEndingRound = false;
        
        if (p1HealthScript != null) p1HealthScript.ResetHealth();
        if (p2HealthScript != null) p2HealthScript.ResetHealth();
        if (timerScript != null) timerScript.ResetTimer();

        if (koRoot != null) koRoot.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        UpdateRoundWinIcons();
    }

    void Update()
    {
        if (!isRoundActive || isEndingRound) return;

        if (p1HealthScript == null || p2HealthScript == null || timerScript == null)
        {
            Debug.LogWarning("RoundManager: manque des refs la");
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

        if (timerScript != null) timerScript.StopTimer(); 
        
        if (FightManager.Instance != null) FightManager.Instance.SetPlayersControl(false);

        if (winner == 1) p1Wins++;
        if (winner == 2) p2Wins++;

        UpdateRoundWinIcons();

        Debug.Log("Round win : " + winner);
        StartCoroutine(RoundEndSequence(winner));
    }

    private IEnumerator RoundEndSequence(int winner)
    {
        Time.timeScale = slowMoScale;
        yield return new WaitForSecondsRealtime(slowMoDuration);
        Time.timeScale = 1f;

        yield return StartCoroutine(PlayKoEffect());
        ShowWinnerPanel(winner);
        bool matchFinished = (p1Wins >= roundsToWin || p2Wins >= roundsToWin);

        if (matchFinished)
        {
            if (endWinText != null) endWinText.text = GetWinnerDisplayName(winner) + " est le gagnant de ce match!";
            
            foreach (PlayerRage rage in FindObjectsOfType<PlayerRage>()) rage.ResetForMatch();

            yield return new WaitForSecondsRealtime(endMenuDelay);

            if (winPanel != null) winPanel.SetActive(false);
            if (endPanel != null) endPanel.SetActive(true);

            isEndingRound = false;
            yield break;
        }

        yield return new WaitForSecondsRealtime(winPanelDurationInRound);

        if (winPanel != null) winPanel.SetActive(false);

        ResetNewRound();
        isRoundActive = true;
        isEndingRound = false;
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

    private void ResetNewRound()
    {
        if (FightManager.Instance != null) FightManager.Instance.ResetPlayersForNewRound();

        if (p1HealthScript != null) p1HealthScript.ResetHealth(); 
        if (p2HealthScript != null) p2HealthScript.ResetHealth();
        
        if (timerScript != null) timerScript.ResetTimer();
    }

    private void ShowWinnerPanel(int winner)
    {
        if (winPanel != null) winPanel.SetActive(true);

        if (winText != null)
        {
            if (winner == 0) winText.text = "égalité"; // egalite
            else winText.text = GetWinnerDisplayName(winner) + " a gagné ce round!";
        }
    }

    private void UpdateRoundWinIcons()
    {
        for (int i = 0; i < p1RoundsWinIcons.Length; i++)
        {
            if (p1RoundsWinIcons[i] != null) p1RoundsWinIcons[i].SetActive(i < p1Wins);
        }
        for (int i = 0; i < p2RoundsWinIcons.Length; i++)
        {
            if (p2RoundsWinIcons[i] != null) p2RoundsWinIcons[i].SetActive(i < p2Wins);
        }
    }

    private string GetWinnerDisplayName(int winner)
    {
        if (winner == 1)
        {
            if (GameManagerSelect.Instance != null && !string.IsNullOrEmpty(GameManagerSelect.Instance.firstSelectedName))
            {
                return GameManagerSelect.Instance.firstSelectedName;
            }

            return "Player 1";
        }

        if (winner == 2)
        {
            if (GameManagerSelect.Instance != null && !string.IsNullOrEmpty(GameManagerSelect.Instance.secondSelectedName))
            {
                return GameManagerSelect.Instance.secondSelectedName;
            }

            return "Player 2";
        }
        return "Nobody";
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeCharacter()
    {
        Time.timeScale = 1f;
        if (GameManagerSelect.Instance != null)
        {
            GameManagerSelect.Instance.firstSelectedPrefab = null;
            GameManagerSelect.Instance.secondSelectedPrefab = null;
            GameManagerSelect.Instance.firstSelectedName = "";
            GameManagerSelect.Instance.secondSelectedName = "";
        }
        RealMenuManager.openStoryPanelOnLoad = false;
        RealMenuManager.open1v1PanelOnLoad = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}