using UnityEngine;
using TMPro;

public class SelectedCharacterName : MonoBehaviour
{
    [SerializeField] private TMP_Text player1NameText;
    [SerializeField] private TMP_Text player2NameText;
    private void Start()
    {
        if (GameManagerSelect.Instance == null)
        {
            Debug.LogError("GameManagerSelect pas la");
            return;
        }
        if (player1NameText != null) player1NameText.text = GameManagerSelect.Instance.firstSelectedName;
        if (player2NameText != null) player2NameText.text = GameManagerSelect.Instance.secondSelectedName;
    }
}
