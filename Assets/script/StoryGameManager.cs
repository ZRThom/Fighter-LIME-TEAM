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
        }
    }

    IEnumerator WaitAndResetPositions()
    {
        yield return new WaitForSecondsRealtime(2f);
        ResetPositions();
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
