using UnityEngine;

public class GameManagerSelect : MonoBehaviour
{
    public static GameManagerSelect Instance;

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
        Debug.Log($"Player1Prefab = {GameManagerSelect.Instance.player1Prefab?.name}, Player2Prefab = {GameManagerSelect.Instance.player2Prefab?.name}");
    }

    public void SelectPlayer(int playerNumber, GameObject prefab)
    {
        if (playerNumber == 1)
        {
            firstSelectedPrefab = prefab;
            player1Prefab = prefab;
        }
        else if (playerNumber == 2)   
        {
            secondSelectedPrefab = prefab;
            player2Prefab = prefab;
        }
        Debug.Log($"Player {playerNumber} selected: {prefab.name}");
    }
}
