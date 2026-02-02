using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public int player1Choice = -1;
    public int player2Choice = -1;

    // Assigné aux boutons dans l'inspecteur
    public void SelectPlayer1(int index)
    {
        player1Choice = index;
        Debug.Log("Player1 choisi : " + index);
    }

    public void SelectPlayer2(int index)
    {
        player2Choice = index;
        Debug.Log("Player2 choisi : " + index);
    }
}
