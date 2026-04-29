using UnityEngine;
using UnityEngine.EventSystems;

public class MapSelect : MonoBehaviour, ISubmitHandler
{
    public GameObject selectionMark;
    public GameObject mapPrefab;
    public static MapSelect currentSelected;

    public void OnPointerDown(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        SelectMap();
    }

    public void OnSubmit(BaseEventData ventData)
    {
        SelectMap();
    }

    public void SelectMap()
    {
        if (currentSelected != null)
        {
            currentSelected.selectionMark.SetActive(false);
        }
        selectionMark.SetActive(true);
        currentSelected = this;

        GameManagerSelect.Instance.SelectMap(mapPrefab);
    }
}
