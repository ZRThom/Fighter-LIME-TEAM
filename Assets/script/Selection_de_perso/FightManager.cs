using UnityEngine;

public class FightManager : MonoBehaviour
{
    public Transform leftSpawn; // left
    public Transform rightSpawn; // right
    public CameraFollow2d cameraFollow;
    void Start()
    {
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        if (GameManager.Instance.firstSelectedPrefab == null || GameManager.Instance.secondSelectedPrefab == null)
        {
            Debug.LogError("two players have to be select !");
            return;
        }

        GameObject leftPlayer = Instantiate(GameManager.Instance.firstSelectedPrefab, leftSpawn.position, Quaternion.identity);
        leftPlayer.name = "PlayerLeft";

        GameObject rightPlayer = Instantiate(GameManager.Instance.secondSelectedPrefab, rightSpawn.position, Quaternion.identity);
        rightPlayer.name = "PlayerRight";

        if (cameraFollow != null)
        {
            cameraFollow.player = leftPlayer.transform;
            cameraFollow.player2 = rightPlayer.transform;
        }
    }
}
