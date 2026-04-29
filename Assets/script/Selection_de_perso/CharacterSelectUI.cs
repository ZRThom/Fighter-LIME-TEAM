using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelectUI : MonoBehaviour, ISubmitHandler, IPointerDownHandler
{
    [Header("Selection Marks")]
    public GameObject selectionMarkP1;
    public GameObject selectionMarkP2;

    [Header("Character")]
    public GameObject characterPrefab;
    public int characterID = 0;

    [Header("Visual Feedback")]
    public Image frameGlow;

    private static CharacterSelectUI p1Selected;
    private static CharacterSelectUI p2Selected;

    private void Start()
    {
        // perso 1, 0 debloqué de base
        if (characterID == 0 || characterID == 1)
        {
            PlayerPrefs.SetInt("Purchased_Char_" + characterID, 1);
            PlayerPrefs.Save();
        }

        if (selectionMarkP1 != null)
            selectionMarkP1.SetActive(false);

        if (selectionMarkP2 != null)
            selectionMarkP2.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            TrySelectCharacter(1);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            TrySelectCharacter(2);
        }

        if (characterPrefab != null)
            Debug.Log($"Click {eventData.button} on {characterPrefab.name}");
    }

    public void OnSubmit(BaseEventData eventData)
    {
        // Submit clavier/manette = sélection joueur 1
        TrySelectCharacter(1);

        if (characterPrefab != null)
            Debug.Log($"Controller P1 on {characterPrefab.name}");
    }

    private void TrySelectCharacter(int playerNumber)
    {
        if (characterPrefab == null)
        {
            Debug.LogWarning("Character prefab manquant.");
            return;
        }

        bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterID, 0) == 1;

        if (!isPurchased)
        {
            StopAllCoroutines();
            StartCoroutine(FlashColor(Color.red));
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FlashColor(Color.green));

        SelectCharacter(playerNumber);
    }

    private void SelectCharacter(int playerNumber)
    {
        if (playerNumber == 1)
        {
            if (p1Selected != null && p1Selected.selectionMarkP1 != null)
                p1Selected.selectionMarkP1.SetActive(false);

            if (selectionMarkP1 != null)
                selectionMarkP1.SetActive(true);

            p1Selected = this;

            GameManagerSelect.Instance.SelectPlayer(1, characterPrefab);
        }
        else if (playerNumber == 2)
        {
            if (p2Selected != null && p2Selected.selectionMarkP2 != null)
                p2Selected.selectionMarkP2.SetActive(false);

            if (selectionMarkP2 != null)
                selectionMarkP2.SetActive(true);

            p2Selected = this;

            GameManagerSelect.Instance.SelectPlayer(2, characterPrefab);
        }
    }

    private IEnumerator FlashColor(Color targetColor)
    {
        if (frameGlow == null)
            yield break;

        frameGlow.color = targetColor;
        yield return new WaitForSecondsRealtime(0.2f);
        frameGlow.color = Color.white;
    }

    public void OnCharacterSelected(BaseEventData data)
    {
        PointerEventData eventData = data as PointerEventData;

        if (eventData == null)
            return;

        OnPointerDown(eventData);
    }
}