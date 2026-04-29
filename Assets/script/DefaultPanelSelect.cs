using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DefaultPanelSelect : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    private void OnEnable()
    {
        StartCoroutine(SelectNext());
    }

    private IEnumerator SelectNext()
    {
        yield return null;
        if (EventSystem.current == null) yield break;
        EventSystem.current.SetSelectedGameObject(null);
        if (firstSelectedButton != null) EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}