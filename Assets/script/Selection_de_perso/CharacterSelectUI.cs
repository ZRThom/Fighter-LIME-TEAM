using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectUI : MonoBehaviour, ISubmitHandler
{
    public GameObject selectionMarkP1;
    public GameObject selectionMarkP2;
    public GameObject characterPrefab;
    private static CharacterSelectUI p1Selected;
    private static CharacterSelectUI p2Selected;

    public void OnPointerDown(BaseEventData Data)
    {
        PointerEventData eventData = (PointerEventData)Data;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (p1Selected != null) p1Selected.selectionMarkP1.SetActive(false);

            selectionMarkP1.SetActive(true);
            p1Selected = this;

            GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (p2Selected != null) p2Selected.selectionMarkP2.SetActive(false);

            selectionMarkP2.SetActive(true);
            p2Selected = this;

            GameManagerSelect.Instance.SelectPlayer(2, characterPrefab);
        }
        Debug.Log($"Click {eventData.button} on {characterPrefab.name}");
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (p1Selected != null) p1Selected.selectionMarkP1.SetActive(false);

        selectionMarkP1.SetActive(true);
        p1Selected = this;
        GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
        Debug.Log($"controller P1 on {characterPrefab.name}");
        
    }
}