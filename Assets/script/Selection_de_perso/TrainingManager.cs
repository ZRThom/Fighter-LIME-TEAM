using UnityEngine;
using System.Collections;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance;

    [Header("Spawn Settings")]
    public Transform playerSpawnPoint; // Le point de spawn du joueur
    public Transform dummyTransform;   // Le transform du mannequin déjà présent
    
    [Header("Camera")]
    public CameraFollow2d cameraFollow;

    [Header("Audio")]
    public AudioClip trainingMusic;

    [HideInInspector] public GameObject player1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Invoke(nameof(SpawnPlayer), 0.1f);
        
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.isTraining = true;
        }
        
        if (trainingMusic != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(trainingMusic);
        }
    }

    public void SpawnPlayer()
    {
        if (GameManagerSelect.Instance == null || GameManagerSelect.Instance.firstSelectedPrefab == null)
        {
            Debug.LogError("Le joueur n'a pas été sélectionné !");
            return;
        }

        GameObject spawnedPlayer = Instantiate(GameManagerSelect.Instance.firstSelectedPrefab, playerSpawnPoint.position, Quaternion.identity);
        spawnedPlayer.name = "PlayerTraining";
        
        player1 = spawnedPlayer;

        PlayerConfig pConfig = spawnedPlayer.GetComponent<PlayerConfig>();
        if (pConfig != null)
        {
            pConfig.playerNumber = 1;
            pConfig.opponentTransform = dummyTransform;
        }

        PlayerHealth pHealth = spawnedPlayer.GetComponent<PlayerHealth>();
        if (pHealth != null) pHealth.playerID = 1;

        if (cameraFollow != null)
        {
            cameraFollow.player = spawnedPlayer.transform;
            cameraFollow.player2 = dummyTransform;
        }
    }
}