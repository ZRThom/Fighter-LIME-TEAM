using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelectUI : MonoBehaviour, ISubmitHandler
{
    public GameObject selectionMarkP1;
    public GameObject selectionMarkP2;
    public GameObject characterPrefab;
    public Image frameGlow; 
    public int characterID = 0; 

    private void Start()
    {

        if (characterID == 0 || characterID == 1)
        {
            PlayerPrefs.SetInt("Purchased_Char_" + characterID, 1);
            PlayerPrefs.Save();
        }
    }

    public void OnCharacterSelected(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;
        
        bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterID, 0) == 1;

        if (!isPurchased)
        {
            StopAllCoroutines();
            StartCoroutine(FlashColor(Color.red));
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FlashColor(Color.green));

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ResetOtherMarks(1);
            selectionMarkP1.SetActive(true);
            GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ResetOtherMarks(2);
            selectionMarkP2.SetActive(true);
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