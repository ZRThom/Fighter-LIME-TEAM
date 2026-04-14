using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private Timer timer;

    private int round = 0;
    private int p1score = 0;
    private int p2score = 0;
    private bool isRoundActive = true;

    [Header("Selection")]
    public CharacterSelection selection;

    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;

    [Header("Player Spawns")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [SerializeField] public GameObject player1;
    [SerializeField] public GameObject player2;

    void Start()
    {
        timer = FindFirstObjectByType<Timer>();
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        //  JOUEUR 1 
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

        // JOUEUR 2 
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

        //  CONFIGURATION J
        ConfigurerJoueurs();

        // LIAISON CAMÉRA 
        CameraFollow2d scriptCamera = Camera.main.GetComponent<CameraFollow2d>();
        if (scriptCamera != null)
        {
            scriptCamera.player = player1.transform;
            scriptCamera.player2 = player2.transform;
        }
    }

    private void ConfigurerJoueurs()
    {
        if (player1 == null || player2 == null) return;

        SetLayerRecursively(player1, LayerMask.NameToLayer("Joueur1"));
        SetLayerRecursively(player2, LayerMask.NameToLayer("Joueur2"));

        PlayerConfig configP1 = player1.GetComponent<PlayerConfig>();
        PlayerConfig configP2 = player2.GetComponent<PlayerConfig>();
        PlayerHealth healthP1 = player1.GetComponent<PlayerHealth>();
        PlayerHealth healthP2 = player2.GetComponent<PlayerHealth>();

        if (configP1 != null && configP2 != null)
        {
            // Setup J1
            configP1.playerNumber = 1;
            configP1.enemyLayers = LayerMask.GetMask("Joueur2");
            configP1.opponentTransform = player2.transform;
            if(healthP1 != null) healthP1.playerID = 1;

            // Setup J2
            configP2.playerNumber = 2;
            configP2.enemyLayers = LayerMask.GetMask("Joueur1");
            configP2.opponentTransform = player1.transform;
            if(healthP2 != null) healthP2.playerID = 2;
        }
    }

    void endRound()
    {
        isRoundActive = false;
        
        PlayerHealth h1 = player1.GetComponent<PlayerHealth>();
        PlayerHealth h2 = player2.GetComponent<PlayerHealth>();

        if (h1.GetCurrentHealth() <= 0) p2score += 1;
        else if (h2.GetCurrentHealth() <= 0) p1score += 1;
        else if (h1.GetCurrentHealth() > h2.GetCurrentHealth()) p1score += 1;
        else p2score += 1;

        Debug.Log($"Score : P1 {p1score} - P2 {p2score}");

        if (p1score < 2 && p2score < 2)
        {
            resetRound();
        }
        else
        {
            Debug.Log("FIN DU MATCH !");
        }
        round += 1;
    }

    void resetRound()
    {
        player1.transform.position = player1Spawn.position;
        player2.transform.position = player2Spawn.position;

        player1.GetComponent<PlayerHealth>().currentHealth = player1.GetComponent<PlayerHealth>().maxHealth;
        player2.GetComponent<PlayerHealth>().currentHealth = player2.GetComponent<PlayerHealth>().maxHealth;
        
        player1.SetActive(true);
        player2.SetActive(true);

        timer.ResetTimer();
        isRoundActive = true;
    }

    void Update()
    {
        if (player1 == null || player2 == null || !isRoundActive) return;

        int hp1 = player1.GetComponent<PlayerHealth>().GetCurrentHealth();
        int hp2 = player2.GetComponent<PlayerHealth>().GetCurrentHealth(); 

        if (timer.timeLeft <= 0 || hp1 <= 0 || hp2 <= 0)
        {
            endRound();
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}