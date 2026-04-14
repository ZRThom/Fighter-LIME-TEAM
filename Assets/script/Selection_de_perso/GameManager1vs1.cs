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

        player1 = PlayerInput.Instantiate(
            characterPrefabs[p1],
            controlScheme: "Keyboard&Mouse",
            pairWithDevice: Keyboard.current
        ).gameObject;
        player1.transform.position = player1Spawn.position;
        player1.name = "Player1";

        InputDevice deviceForPlayer2 = Gamepad.current != null ? (InputDevice)Gamepad.current : Keyboard.current;
        player2 = PlayerInput.Instantiate(
            characterPrefabs[p2],
            controlScheme: deviceForPlayer2 is Gamepad ? "Gamepad" : "Keyboard&Mouse",
            pairWithDevice: deviceForPlayer2
        ).gameObject;
        player2.transform.position = player2Spawn.position;
        player2.name = "Player2";

        ConfigurerJoueurs();

        CameraFollow2d scriptCamera = Camera.main.GetComponent<CameraFollow2d>();
        if (scriptCamera != null)
        {
            scriptCamera.player = player1.transform;
            scriptCamera.player2 = player2.transform;
        }

        Debug.Log("Début du combat !");
    }

    private void ConfigurerJoueurs()
    {
        int layerJ1 = LayerMask.NameToLayer("Joueur1");
        int layerJ2 = LayerMask.NameToLayer("Joueur2");

        SetLayerRecursively(player1, layerJ1);
        SetLayerRecursively(player2, layerJ2);

        PlayerConfig configP1 = player1.GetComponent<PlayerConfig>();
        PlayerConfig configP2 = player2.GetComponent<PlayerConfig>();

        if (configP1 != null && configP2 != null)
        {
            // --- Joueur 1 ---
            configP1.playerNumber = 1;
            configP1.enemyLayers = LayerMask.GetMask("Joueur2");
            configP1.opponentTransform = player2.transform;

            // --- Joueur 2 ---
            configP2.playerNumber = 2;
            configP2.enemyLayers = LayerMask.GetMask("Joueur1");
            configP2.opponentTransform = player1.transform;
        }
        else
        {
            Debug.LogWarning("Attention : Il manque le composant PlayerConfig sur un de tes personnages !");
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}