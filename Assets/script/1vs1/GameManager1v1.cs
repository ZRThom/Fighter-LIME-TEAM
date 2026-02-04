//using UnityEngine;
//using UnityEngine.InputSystem;
//
//public class GameManager1vs1 : MonoBehaviour
//{
//    [Header("Player Spawns")]
//    public Transform player1Spawn;
//    public Transform player2Spawn;
//
//    private GameObject player1;
//    private GameObject player2;
//
//    void Start()
//    {
//        SpawnPlayers();
//    }
//
//    void SpawnPlayers()
//    {
//        // Player 1
//        if (GameManager.Instance.player1Prefab != null)
//        {
//            player1 = PlayerInput.Instantiate(
//                GameManager.Instance.player1Prefab,
//                controlScheme: "Keyboard&Mouse",
//                pairWithDevice: Keyboard.current
//            ).gameObject;
//
//            player1.transform.position = player1Spawn.position;
//            player1.name = "Player1";   
//        }
//
//        // Player 2
//        if (GameManager.Instance.player2Prefab != null)
//        {
//            InputDevice deviceForPlayer2 = Gamepad.current != null ? (InputDevice)Gamepad.current : Keyboard.current;
//
//            player2 = PlayerInput.Instantiate(
//                GameManager.Instance.player2Prefab,
//                controlScheme: deviceForPlayer2 is Gamepad ? "Gamepad" : "Keyboard&Mouse",
//                pairWithDevice: deviceForPlayer2
//            ).gameObject;
//
//            player2.transform.position = player2Spawn.position;
//            player2.name = "Player2";
//        }
//    }
//}
//