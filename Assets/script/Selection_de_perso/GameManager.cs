using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Character Select")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject firstSelectedPrefab;
    public GameObject secondSelectedPrefab;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log($"Player1Prefab = {GameManager.Instance.player1Prefab?.name}, Player2Prefab = {GameManager.Instance.player2Prefab?.name}");
    }

    public void SelectPlayer(int playerNumber, GameObject prefab)
    {
        if (firstSelectedPrefab == null)
        {
            firstSelectedPrefab = prefab;
        }
        else
        {
            secondSelectedPrefab = prefab;
        }
        if (playerNumber == 1) player1Prefab = prefab;
        else player2Prefab = prefab;
        Debug.Log($"Player {playerNumber} selected: {prefab.name}");
    }
}
