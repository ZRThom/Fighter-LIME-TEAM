using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Timer timer;

    private int round = 0;
    private int p1score = 0;
    private int p2score = 0;

    private int roundtemp = 0;
    private bool isRoundActive = true;



    [Header("Selection")]
    public CharacterSelection selection;

    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;

    [Header("Player Spawns")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [SerializeField]
    public GameObject player1;
    [SerializeField]
    public GameObject player2;

    void Start()
    {
        timer = FindFirstObjectByType<Timer>();
        SpawnPlayers();

    }

    void SpawnPlayers()
    {
        // Joueur 1
        int index1 = selection.player1Choice;
        if (index1 >= 0 && index1 < characterPrefabs.Length)
        {
            player1 = PlayerInput.Instantiate(
                characterPrefabs[index1],
                controlScheme: "Keyboard&Mouse",
                pairWithDevice: Keyboard.current
            ).gameObject;

            player1.transform.position = player1Spawn.position;
            player1.name = "Player1";
        }

        // Joueur 2
        int index2 = selection.player2Choice;
        if (index2 >= 0 && index2 < characterPrefabs.Length)
        {
            InputDevice deviceForPlayer2 = Gamepad.current != null ? (InputDevice)Gamepad.current : Keyboard.current;

            player2 = PlayerInput.Instantiate(
                characterPrefabs[index2],
                controlScheme: deviceForPlayer2 is Gamepad ? "Gamepad" : "Keyboard&Mouse",
                pairWithDevice: deviceForPlayer2
            ).gameObject;

            player2.transform.position = player2Spawn.position;
            player2.name = "Player2";
        }
    }

    void endRound()
    {
        isRoundActive = false;
        // Le joueur 1 est K.O. (le joueur 2 gagne)
        if (player1.GetComponent<PlayerHealth>().GetCurrentHealth() <= 0)
        {
            p2score += 1;
        }
        // Le joueur 2 est K.O. (le joueur 1 gagne)
        else if (player2.GetComponent<PlayerHealth>().GetCurrentHealth() <= 0)
        {
            p1score += 1;
        }
        // Personne n'est mort, le temps est fini, on compare les PV
        else if (player1.GetComponent<PlayerHealth>().GetCurrentHealth() > player2.GetComponent<PlayerHealth>().GetCurrentHealth())
        {
            p1score += 1;
        }
        else
        {
            p2score += 1;
        }
        if (p1score < 2 && p2score < 2)
        {
            resetRound();
            isRoundActive = true;
        }
        else
        {
            Debug.Log("FIN DU MATCH ! Le gagnant est affiché.");
        }

        round += 1;
    }

    void resetRound()
    {
        //remettre les PV au max et relancer le timer

        player1.GetComponent<PlayerHealth>().currentHealth = player1.GetComponent<PlayerHealth>().maxHealth;
        player2.GetComponent<PlayerHealth>().currentHealth = player2.GetComponent<PlayerHealth>().maxHealth;
        timer.ResetTimer();
        player1.SetActive(true);
        player2.SetActive(true);

    }

    void Update()
    {
        int hp1 = player1.GetComponent<PlayerHealth>().GetCurrentHealth();
        int hp2 = player2.GetComponent<PlayerHealth>().GetCurrentHealth(); 
        // On vérifie les conditions
        if ((timer.timeLeft <= 0 || hp1 <= 0 || hp2 <= 0) && isRoundActive)
        {
            endRound();
        }
    }
}
