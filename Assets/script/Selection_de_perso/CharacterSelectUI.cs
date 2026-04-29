using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelectUI : MonoBehaviour
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
    }

    void ResetOtherMarks(int playerNumber)
    {
        CharacterSelectUI[] allButtons = FindObjectsOfType<CharacterSelectUI>();
        
        foreach (CharacterSelectUI btn in allButtons)
        {
            if (playerNumber == 1 && btn.selectionMarkP1 != null)
                btn.selectionMarkP1.SetActive(false);
            else if (playerNumber == 2 && btn.selectionMarkP2 != null)
                btn.selectionMarkP2.SetActive(false);
        }
    }

    IEnumerator FlashColor(Color targetColor)
    {
        if (frameGlow == null) yield break;
        frameGlow.color = targetColor;
        yield return new WaitForSecondsRealtime(0.2f);
        frameGlow.color = Color.white;
    }
}