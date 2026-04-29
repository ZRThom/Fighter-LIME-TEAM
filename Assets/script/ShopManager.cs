using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("ORDRE : 0:Enginio, 1:Blob, 2:Jojo, 3:Monstre, 4:Boss")]
    public List<Image> characterImages;

    void Start() 
    { 
        PlayerPrefs.SetInt("Purchased_Char_0", 1);
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

            bool isPurchased = PlayerPrefs.GetInt("Purchased_Char_" + i, 0) == 1;
            TextMeshProUGUI[] textComponents = characterImages[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            
            foreach (TextMeshProUGUI txt in textComponents)
            {
                if (txt.text.Length > 5) 
                {
                    if (isPurchased)
                    {
                        txt.text = "PERSONNAGE DÉVERROUILLÉ";
                    }
                    else
                    {
                        txt.color = Color.white;
                        if (i == 2) txt.text = "CONDITION DE DÉBLOCAGE : FINIR LE STAGE 2";
                        else if (i == 3) txt.text = "CONDITION DE DÉBLOCAGE : FINIR LE STAGE 3";
                        else if (i == 4) txt.text = "CONDITION DE DÉBLOCAGE : FINIR LE STAGE 4";
                    }
                }
            }
        }
    }

    public void OnCharacterClick(int id)
    {
        if (PlayerPrefs.GetInt("Purchased_Char_" + id, 0) == 1) return;

        int progression = PlayerPrefs.GetInt("DernierStageFini", 0);

        if (progression >= id)
        {
            PlayerPrefs.SetInt("Purchased_Char_" + id, 1);
            PlayerPrefs.Save();
            UpdateButtons();
        }
    }
}