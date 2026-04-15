using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private PlayerHealth p1;
    private PlayerHealth p2;
    private Timer timer;

    private int p1Score = 0;
    private int p2Score = 0;
    private bool isRoundActive = true;

    void Start() // test mis avril
    {
        StartCoroutine(Init());
    }

    System.Collections.IEnumerator Init()
    {
        yield return new WaitUntil(() => FightManager.Instance.player1 != null);

        p1 = FightManager.Instance.player1.GetComponent<PlayerHealth>();
        p2 = FightManager.Instance.player2.GetComponent<PlayerHealth>();

        timer = FindObjectOfType<Timer>();
    }

    void Update()
    {
        if (!isRoundActive) return;
        if (p1 == null || p2 == null || timer == null) return;
        // conditions
        if (timer.timeLeft <= 0 || p1.GetCurrentHealth() <= 0 || p2.GetCurrentHealth() <= 0)
        {
            endRound();
        }
    }

    void endRound()
    {
        isRoundActive = false;
        // J1 KO : J2 win
        if (p1.GetCurrentHealth() <= 0) p2Score++;
        // J2 KO : J1 win
        else if (p2.GetCurrentHealth() <= 0) p1Score++;
        // PV check is nobody KO
        else if (p1.GetCurrentHealth() > p2.GetCurrentHealth()) p1Score++;
        else p2Score++;
        
        if (p1Score < 2 && p2Score < 2) Invoke(nameof(resetRound), 2f);
        else Debug.Log("FIN DU MATCH ! Le gagnant est affiché.");
    }

    void resetRound()
    {
        //remettre les PV au max et relancer le timer
        p1.ResetHealth();
        p2.ResetHealth();
        p1.gameObject.SetActive(true);
        p2.gameObject.SetActive(true);
        p1.transform.position = FightManager.Instance.P1SpawnPos;
        p2.transform.position = FightManager.Instance.P2SpawnPos;

        var rb1 = p1.GetComponent<Rigidbody2D>();
        var rb2 = p2.GetComponent<Rigidbody2D>();
        if (rb1) rb1.linearVelocity = Vector2.zero;
        if (rb2) rb2.linearVelocity = Vector2.zero;

        timer.ResetTimer();
        isRoundActive = true;
        Debug.Log("reset round");
    }
}
