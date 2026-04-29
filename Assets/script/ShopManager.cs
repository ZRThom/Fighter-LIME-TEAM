using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Glissez les images des boutons ici")]
    public List<Image> characterImages;

    void Start() 
    { 
        PlayerPrefs.SetInt("Purchased_Char_1", 1);
        PlayerPrefs.Save();

        UpdateButtons(); 
    }
    
    private void OnEnable() { UpdateButtons(); }

    public void UpdateButtons()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            if (characterImages[i] == null) continue;

            int characterNumber = i + 1;
            bool isAlreadyPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterNumber, 0) == 1;

            Color targetColor = isAlreadyPurchased ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f);
            ApplyLook(characterImages[i], targetColor);

            if (isAlreadyPurchased)
            {
                TextMeshProUGUI[] textComponents = characterImages[i].GetComponentsInChildren<TextMeshProUGUI>(true);
                
                foreach (TextMeshProUGUI txt in textComponents)
                {
                    if (txt.text.Contains("CONDITION") || txt.text.Contains("STAGE") || txt.text.Length > 10) 
                    {
                        txt.text = "PERSONNAGE DÉVERROUILLÉ";
                    }
                }
            }
        }
    }

    private void ApplyLook(Image mainImg, Color color)
    {
        mainImg.color = color;
        Image[] children = mainImg.GetComponentsInChildren<Image>(true);
        foreach (Image img in children)
        {
            img.color = color;
        }
    }

    public void OnCharacterClick(int characterNumber)
    {
        int progression = PlayerPrefs.GetInt("DernierStageFini", 0);
        bool isAlreadyPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterNumber, 0) == 1;

        if (isAlreadyPurchased) return;

        if (progression >= characterNumber)
        {
            PlayerPrefs.SetInt("Purchased_Char_" + characterNumber, 1);
            PlayerPrefs.Save();
            
            StopAllCoroutines();
            StartCoroutine(FlashGreenEffect(characterImages[characterNumber - 1]));
        }
        else
        {
            Debug.Log("Stage trop bas pour débloquer ce perso !");
            StopAllCoroutines(); 
            StartCoroutine(FlashRedEffect(characterImages[characterNumber - 1]));
        }
    }

    IEnumerator FlashRedEffect(Image targetImage)
    {
        for (int i = 0; i < 2; i++)
        {
            ApplyLook(targetImage, Color.red);
            yield return new WaitForSecondsRealtime(0.1f);
            ApplyLook(targetImage, new Color(0.7f, 0.7f, 0.7f, 0.8f));
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    IEnumerator FlashGreenEffect(Image targetImage)
    {
        for (int i = 0; i < 3; i++)
        {
            ApplyLook(targetImage, Color.green);
            yield return new WaitForSecondsRealtime(0.08f);
            ApplyLook(targetImage, Color.white);
            yield return new WaitForSecondsRealtime(0.08f);
        }
        UpdateButtons();
    }
}