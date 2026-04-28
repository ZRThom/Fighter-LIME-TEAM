using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    public GameObject selectionMarkP1;
    public GameObject selectionMarkP2;
    public GameObject characterPrefab;

    [Header("Unlock Settings")]
    public int characterID = 0; // 0 for Enginio, 1-4 for Bosses

    private void Start() 
    { 
        if(characterID == 0) PlayerPrefs.SetInt("Purchased_Char_0", 1);
        UpdateState(); 
    }
    
    private void OnEnable() { UpdateState(); }

    public void UpdateState()
    {
        bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterID, 0) == 1;
        
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.interactable = isPurchased;
        }
    }

    public void OnCharacterSelected(BaseEventData data)
    {
        bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterID, 0) == 1;

        if (!isPurchased)
        {
            Debug.Log("<color=red>ACCESS DENIED:</color> You must buy this character in the Shop first!");
            return;
        }

        Debug.Log("<color=green>ACCESS GRANTED:</color> Character " + gameObject.name + " selected!");

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
}