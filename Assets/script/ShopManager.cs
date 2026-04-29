using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Drag the Parent Buttons here (Persos_Stage_1, etc.)")]
    public List<Image> characterImages;

    void Start() { UpdateButtons(); }
    private void OnEnable() { UpdateButtons(); }

    public void UpdateButtons()
    {
        int progression = PlayerPrefs.GetInt("DernierStageFini", 0);
        
        for (int i = 0; i < characterImages.Count; i++)
        {
            if (characterImages[i] == null) continue;

            int requirement = i + 1;
            bool isAlreadyPurchased = PlayerPrefs.GetInt("Purchased_Char_" + requirement, 0) == 1;

            Color targetColor = isAlreadyPurchased ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.6f);

            ApplyLook(characterImages[i], targetColor, isAlreadyPurchased);
        }
    }

    private void ApplyLook(Image mainImg, Color color, bool purchased)
    {
        Transform chainsTransform = mainImg.transform.Find("Chains");
        if (chainsTransform != null)
        {
            chainsTransform.gameObject.SetActive(!purchased);
        }

        mainImg.color = color;
        Image[] children = mainImg.GetComponentsInChildren<Image>(true);
        foreach (Image img in children)
        {
            if (img.gameObject.name != "Chains" && img.transform.parent.name != "Chains")
            {
                img.color = color;
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
        for (int i = 0; i < 2; i++)
        {
            ApplyLook(targetImage, Color.red, false);
            yield return new WaitForSecondsRealtime(0.1f);
            ApplyLook(targetImage, new Color(0.3f, 0.3f, 0.3f, 0.6f), false);
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    IEnumerator FlashGreenEffect(Image targetImage)
    {
        for (int i = 0; i < 3; i++)
        {
            ApplyLook(targetImage, Color.green, true);
            yield return new WaitForSecondsRealtime(0.08f);
            ApplyLook(targetImage, Color.white, true);
            yield return new WaitForSecondsRealtime(0.08f);
        }
        UpdateButtons();
    }
}