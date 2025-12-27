using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager1vs1 : MonoBehaviour
{
    [Header("Selection")]
    public CharacterSelection selection;

    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;

    [Header("Player Spawns")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [HideInInspector]
    public GameObject player1;
    [HideInInspector]
    public GameObject player2;

    void Start()
    {
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        // Joueur 1
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

        // Joueur 2
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
    }
}
