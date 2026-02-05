using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectUI : MonoBehaviour
{
    public GameObject selectionMarkP1;
    public GameObject selectionMarkP2;
    
    private static CharacterSelectUI p1Selected;
    private static CharacterSelectUI p2Selected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (p1Selected != null)
            {
                p1Selected.selectionMarkP1.SetActive(false);
            }
            selectionMarkP1.SetActive(true);
            p1Selected = this;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (p2Selected != null)
            {
                p2Selected.selectionMarkP2.SetActive(false);
            }
            selectionMarkP2.SetActive(true);
            p2Selected = this;
        }
    }
}
