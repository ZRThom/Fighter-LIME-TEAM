using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighlightControllerButton : MonoBehaviour
{
    private Button button;
    void Awake()
    {
        button = GetComponent<Button>();
    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            var colors = button.colors;
            button.image.color = colors.highlightedColor;
        }
        else
        {
            var colors = button.colors;
            button.image.color = colors.normalColor;
        }
    }
}
