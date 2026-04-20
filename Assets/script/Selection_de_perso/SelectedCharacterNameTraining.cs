using UnityEngine;
using TMPro;

public class SelectedCharacterNameTraining : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    private void Start()
    {
        if (GameManagerSelect.Instance == null)
        {
            Debug.LogError("GameManagerSelect n'est pas présent");
            return;
        }
        if (playerNameText != null)
        {
            playerNameText.text = GameManagerSelect.Instance.firstSelectedName;
        }
    }
}