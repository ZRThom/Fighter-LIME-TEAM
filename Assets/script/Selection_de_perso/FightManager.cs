using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    public Transform leftSpawn; // left
    public Transform rightSpawn; // right
    public CameraFollow2d cameraFollow;

    public Vector3 P1SpawnPos => leftSpawn.position;
    public Vector3 P2SpawnPos => rightSpawn.position;

    [HideInInspector] public GameObject player1;
    [HideInInspector] public GameObject player2;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Invoke(nameof(SpawnPlayers), 0.1f);
    }

    void SpawnPlayers()
    {
        if (GameManagerSelect.Instance.firstSelectedPrefab == null || GameManagerSelect.Instance.secondSelectedPrefab == null)
        {
            Debug.LogError("two players have to be select !");
            return;
        }

        GameObject leftPlayer = Instantiate(GameManagerSelect.Instance.firstSelectedPrefab, leftSpawn.position, Quaternion.identity);
        leftPlayer.name = "PlayerLeft";
        var leftConfig = leftPlayer.GetComponent<PlayerConfig>();
        leftConfig.playerNumber = 1;
        var leftHealth = leftPlayer.GetComponent<PlayerHealth>();
        if (leftHealth) leftHealth.playerID = 1;

        GameObject rightPlayer = Instantiate(GameManagerSelect.Instance.secondSelectedPrefab, rightSpawn.position, Quaternion.identity);
        rightPlayer.name = "PlayerRight";
        var rightConfig = rightPlayer.GetComponent<PlayerConfig>();
        rightConfig.playerNumber = 2;
        var rightHealth = rightPlayer.GetComponent<PlayerHealth>();
        if (rightHealth) rightHealth.playerID = 2;

        // right player flip
        // Vector3 scale = rightPlayer.transform.localScale;
        // scale.x *= -1f;
        // rightPlayer.transform.localScale = scale;

        leftConfig.opponentTransform = rightPlayer.transform;
        rightConfig.opponentTransform = leftPlayer.transform;

        player1 = leftPlayer;
        player2 = rightPlayer;

        if (cameraFollow != null)
        {
            cameraFollow.player = leftPlayer.transform;
            cameraFollow.player2 = rightPlayer.transform;
        }
    }
}
