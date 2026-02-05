using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager1vs1 : MonoBehaviour
{
    [Header("Sélection de personnages")]
    public CharacterSelection selection;

    [Header("Personnages")]
    public GameObject[] characterPrefabs;

    [Header("Points de spawn")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [HideInInspector]
    public GameObject player1;
    [HideInInspector]
    public GameObject player2;

    public void StartGame()
    {
        int p1 = selection.player1Choice;
        int p2 = selection.player2Choice;

        if (p1 == -1 || p2 == -1)
        {
            Debug.Log("Les deux joueurs doivent choisir un personnage !");
            return;
        }

        // Instancie Player1 avec PlayerInput
        player1 = PlayerInput.Instantiate(
            characterPrefabs[p1],
            controlScheme: "Keyboard&Mouse",
            pairWithDevice: Keyboard.current
        ).gameObject;
        player1.transform.position = player1Spawn.position;
        player1.name = "Player1";

        // Instancie Player2 avec PlayerInput
        InputDevice deviceForPlayer2 = Gamepad.current != null ? (InputDevice)Gamepad.current : Keyboard.current;
        player2 = PlayerInput.Instantiate(
            characterPrefabs[p2],
            controlScheme: deviceForPlayer2 is Gamepad ? "Gamepad" : "Keyboard&Mouse",
            pairWithDevice: deviceForPlayer2
        ).gameObject;
        player2.transform.position = player2Spawn.position;
        player2.name = "Player2";

        Debug.Log("Début du combat !");
    }
}
