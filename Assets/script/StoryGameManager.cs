using UnityEngine;
using System.Collections;

public class StoryGameManager : MonoBehaviour
{
    [Header("Joueurs ")]
    public PlayerHealth p1;
    public PlayerHealth p2;

    [Header("Points d'apparition ")]
    public Transform p1SpawnPos;
    public Transform p2SpawnPos;

    [Header("UI Story")]
    public StoryPanelManager panelManager;

    [Tooltip("Indique quel boss on affronte (1, 2, 3 ou 4)")]
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
            Debug.LogWarning("Attention : Aucun RoundManager trouvé dans la scène !");
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
                // Victoire du Joueur 1 : Affiche le panel "W" correspondant au boss
                if (bossLevel == 1) panelManager.OpenW1();
                else if (bossLevel == 2) panelManager.OpenW2();
                else if (bossLevel == 3) panelManager.OpenW3();
                else if (bossLevel == 4) panelManager.OpenW4();
            }
            else
            {
                // Défaite du Joueur 1 : Affiche le panel "L" correspondant au boss
                if (bossLevel == 1) panelManager.OpenL1();
                else if (bossLevel == 2) panelManager.OpenL2();
                else if (bossLevel == 3) panelManager.OpenL3();
                else if (bossLevel == 4) panelManager.OpenL4();
            }
            
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("ERREUR StoryGameManager : La case 'Panel Manager' est vide dans l'inspecteur ! N'oublie pas d'y glisser ton objet contenant StoryPanelManager.");
        }
    }

    public void ResetPositions()
    {
        if (p1 != null) p1.gameObject.SetActive(true);
        if (p2 != null) p2.gameObject.SetActive(true);

        if (p1 != null && p1SpawnPos != null) p1.transform.position = p1SpawnPos.position;
        if (p2 != null && p2SpawnPos != null) p2.transform.position = p2SpawnPos.position;

        if (p1 != null)
        {
            var rb1 = p1.GetComponent<Rigidbody2D>();
            if (rb1) rb1.linearVelocity = Vector2.zero;
        }
        if (p2 != null)
        {
            var rb2 = p2.GetComponent<Rigidbody2D>();
            if (rb2) rb2.linearVelocity = Vector2.zero;
        }
        Debug.Log("StoryGameManager : Positions réinitialisées.");
    }
}
