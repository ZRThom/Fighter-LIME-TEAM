using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public GameObject characterPrefab;
    public int playerNumber = 1;

    public void SelectPlayer1()
    {
        GameManager.Instance.SelectPlayer(playerNumber, characterPrefab);
    }
}
