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

    public void OnCharacterClick(int characterNumber)
    {
        int progression = PlayerPrefs.GetInt("DernierStageFini", 0);
        bool isAlreadyPurchased = PlayerPrefs.GetInt("Purchased_Char_" + characterNumber, 0) == 1;

        if (isAlreadyPurchased) return;

        if (progression >= characterNumber)
        {
            PlayerPrefs.SetInt("Purchased_Char_" + characterNumber, 1);
            PlayerPrefs.Save();
            
            UpdateButtons();
        }
        else
        {
            Debug.Log("Stage trop bas pour débloquer ce perso !");
        }
    }
}