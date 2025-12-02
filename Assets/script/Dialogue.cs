using UnityEngine;
using System.Collections;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] string[] sentences;

    [SerializeField] float typingSpeed = 0.05f;        
    [SerializeField] float timeBetweenLines = 0.5f;   
    [SerializeField] PlayerController2D playerController;  

    int index = 0;
    bool isTalking;

    HUDManager manager => HUDManager.instance;

    
        public void StartDialogue()
    {
        if (isTalking) return;

        isTalking = true;
        index = 0;

        manager.DialogueHolder.SetActive(true);
        PlayerController2D.instance.CanMove = false;
        PlayerController2D.instance.CanAttack = false;

        StartCoroutine(TypeSentence(sentences[index]));
    }

    public void NextLine()
    {
        if (!isTalking) return;

        manager.ContinueButton.SetActive(false);

        if (index < sentences.Length - 1)
        {
            index++;
            StartCoroutine(TypeSentence(sentences[index]));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        textMeshPro.text = sentence;
        textMeshPro.ForceMeshUpdate();

        int totalChars = textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (counter <= totalChars)
        {
            textMeshPro.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSeconds(typingSpeed);
        }

        manager.ContinueButton.SetActive(true);
    }

    void EndDialogue()
    {
        isTalking = false;
        index = 0;

        manager.DialogueHolder.SetActive(false);
        textMeshPro.text = "";

        PlayerController2D.instance.CanMove = true;
        PlayerController2D.instance.CanAttack = true;
    }
}