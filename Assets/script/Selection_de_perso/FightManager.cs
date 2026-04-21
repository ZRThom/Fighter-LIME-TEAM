using UnityEngine;
using System.Collections;

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

    public void SpawnPlayers()
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

    public void SetPlayersControl(bool enabled)
    {
        SetSinglePlayerControl(player1, enabled);
        SetSinglePlayerControl(player2, enabled);
    }

    private void SetSinglePlayerControl(GameObject player, bool enabled)
    {
        if (player == null) return;
        var input = player.GetComponent<PlayerInputHandler>();
        if (input != null)
        {
            input.SetInputsEnabled(enabled);
            input.ClearInputs();
        }

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null && !enabled)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void ResetPlayersForNewRound()
    {
        if (player1 == null || player2 == null)
        {
            Debug.LogWarning("payers missing, no reset round");
            return;
        }

        ResetSinglePlayer(player1, leftSpawn.position, 1, player2.transform, false);
        ResetSinglePlayer(player2, rightSpawn.position, 2, player1.transform, true);

        if (cameraFollow != null)
        {
            cameraFollow.player = player1.transform;
            cameraFollow.player2 = player2.transform;
        }

        SetPlayersControl(true);
    }

    private void ResetSinglePlayer(GameObject player, Vector3 spawnPos, int playerNumber, Transform opponent, bool faceLeft)
    {
        player.transform.position = spawnPos;
        player.transform.rotation = Quaternion.identity;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        PlayerConfig config = player.GetComponent<PlayerConfig>();
        if (config != null)
        {
            config.playerNumber = playerNumber;
            config.opponentTransform = opponent;
        }

        PlayerInputHandler input = player.GetComponent<PlayerInputHandler>();
        if (input != null)
        {
            input.SetInputsEnabled(true);
            input.ClearInputs();
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
        }
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        Vector3 scale = player.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceLeft ? -1f : 1f);
        player.transform.localScale = scale;
    }
}
