using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections; 

public class CharacterSelectUI : MonoBehaviour
{
    public GameObject selectionMarkP1;
    public GameObject selectionMarkP2;
    public GameObject characterPrefab;

    [Header("Glow Feedback")]
    public Image frameGlow; 

    [Header("Unlock Settings")]
    public int characterID = 0; 

    private void Start() 
    { 
        if(characterID == 0) PlayerPrefs.SetInt("Purchased_Char_0", 1);
        UpdateState(); 
    }
    
    private void OnEnable() { UpdateState(); }

    public void UpdateState()
    {
        bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterID, 0) == 1;
        
    }

    public void OnCharacterSelected(BaseEventData data)
    {
        bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterID, 0) == 1;

        if (!isPurchased)
        {
            StopAllCoroutines(); 
            StartCoroutine(FlashColor(Color.red));
            Debug.Log("BLOQUÉ !");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FlashColor(Color.green));
        Debug.Log("SÉLECTIONNÉ !");

        PointerEventData eventData = (PointerEventData)data;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            selectionMarkP1.SetActive(true);
            GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            selectionMarkP2.SetActive(true);
            GameManagerSelect.Instance.SelectPlayer(2, characterPrefab);
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