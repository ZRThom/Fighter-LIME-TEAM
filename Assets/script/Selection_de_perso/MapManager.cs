using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public Transform mapSpawnPoint;
    public MapRefs currentStage;
    

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnMap();
    }

    void SpawnMap()
    {
        if (GameManagerSelect.Instance == null)
        {
            Debug.Log("no gameManagerSelect found");
            return;
        }

        if (GameManagerSelect.Instance.selectMapPrefab == null)
        {
            Debug.Log("no map select");
            return;
        }

        GameObject mapObj = Instantiate(GameManagerSelect.Instance.selectMapPrefab, mapSpawnPoint.position, Quaternion.identity);
        currentStage = mapObj.GetComponent<MapRefs>();

        if (currentStage == null)
        {
            Debug.LogError("map prefab don't hafve mapRefs");
            return;
        }

        if (FightManager.Instance != null)
        {
            FightManager.Instance.leftSpawn = currentStage.leftSpawn;
            FightManager.Instance.rightSpawn = currentStage.rightSpawn;
        }
    }
}
