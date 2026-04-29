using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectUITraining : MonoBehaviour, ISubmitHandler
{
    public GameObject selectionMark;
    public GameObject characterPrefab;
    private static CharacterSelectUITraining currentSelected;

    public void OnPointerDown(BaseEventData Data)
    {
        PointerEventData eventData = (PointerEventData)Data;
        
        // On accepte le clic gauche pour sélectionner son personnage d'entraînement
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (currentSelected != null) currentSelected.selectionMark.SetActive(false);

            selectionMark.SetActive(true);
            currentSelected = this;

            GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (currentSelected != null) currentSelected.selectionMark.SetActive(false);

        selectionMark.SetActive(true);
        currentSelected = this;
        GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
    }
}