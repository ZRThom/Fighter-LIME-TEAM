//using UnityEngine;
//
//public class TestPlayerSpawn : MonoBehaviour
//{
//    public Transform spawnPoint;
//    void Start()
//    {
//        if (GameManager.Instance.player1Prefab != null)
//        {
//            GameObject p1 = Instantiate(GameManager.Instance.player1Prefab, spawnPoint.position, Quaternion.identity);
//            Debug.Log("Spawned Player1: " + p1.name);
//        }
//
//        if (GameManager.Instance.player2Prefab != null)
//        {
//            GameObject p2 = Instantiate(GameManager.Instance.player2Prefab, spawnPoint.position + Vector3.right * 3f, Quaternion.identity);
//            Debug.Log("Spawned Player2: " + p2.name);
//        }
//    }
//}
