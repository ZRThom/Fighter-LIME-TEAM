using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighlightControllerButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button button;
    private Graphic targetGraphic;
    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null) targetGraphic = button.targetGraphic;
    }

    // update controller (bug panel fix avec defaultPanelSelect) 
    public void OnSelect(BaseEventData eventData)
    {
        ApplyHighlightedColor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ApplyNormalColor();
    }

    private void OnEnable()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            ApplyHighlightedColor();
        }
        else
        {
            ApplyNormalColor();
        }
    }

    private void ApplyHighlightedColor()
    {
        if (button == null || targetGraphic == null) return;
        
        targetGraphic.color = button.colors.highlightedColor;
    }

    private void ApplyNormalColor()
    {
        if (button == null || targetGraphic == null) return;
        
        targetGraphic.color = button.colors.normalColor;
    }
}
