using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Drag the Parent Buttons here (Persos_Stage_1, etc.)")]
    public List<Image> characterImages;

    void Start()
    {
        UpdateButtons();
    }

    private void OnEnable()
    {
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        int progression = PlayerPrefs.GetInt("DernierStageFini", 0);
        
        for (int i = 0; i < characterImages.Count; i++)
        {
            if (characterImages[i] == null) continue;

            int requirement = i + 1;
            bool isUnlocked = (progression >= requirement);
            bool isAlreadyPurchased = PlayerPrefs.GetInt("Purchased_Char_" + requirement, 0) == 1;

            Color targetColor;
            if (isAlreadyPurchased) targetColor = Color.white;
            else if (isUnlocked) targetColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            else targetColor = new Color(0f, 0f, 0f, 1f);

            ApplyColorToButton(characterImages[i], targetColor);
        }
    }

    private void ApplyColorToButton(Image mainImg, Color color)
    {
        mainImg.color = color;
        Image[] children = mainImg.GetComponentsInChildren<Image>();
        foreach (Image img in children)
        {
            img.color = color;
            img.canvasRenderer.SetColor(color);
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
            
            Debug.Log("<color=green>PURCHASE SUCCESS!</color>");
            
            StopAllCoroutines();
            StartCoroutine(FlashGreenEffect(characterImages[characterNumber - 1]));
        }
        else
        {
            StopAllCoroutines(); 
            StartCoroutine(FlashRedEffect(characterImages[characterNumber - 1]));
        }
    }

    IEnumerator FlashRedEffect(Image targetImage)
    {
        Color flashColor = Color.red;
        for (int i = 0; i < 2; i++)
        {
            ApplyColorToButton(targetImage, flashColor);
            yield return new WaitForSecondsRealtime(0.1f);
            ApplyColorToButton(targetImage, Color.black);
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    IEnumerator FlashGreenEffect(Image targetImage)
    {
        Color flashColor = Color.green;
        for (int i = 0; i < 3; i++)
        {
            ApplyColorToButton(targetImage, flashColor);
            yield return new WaitForSecondsRealtime(0.08f);
            ApplyColorToButton(targetImage, Color.white);
            yield return new WaitForSecondsRealtime(0.08f);
        }
        UpdateButtons();
    }
}