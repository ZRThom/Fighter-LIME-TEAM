using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Liste des Boutons de Persos (dans l'ordre 1-5)")]
    public List<Button> boutonsPersos;

    [Header("Liste des Images de Persos (sur les boutons)")]
    public List<Image> imagesPersos;

    void Start()
    {
        // ligne test
        // PlayerPrefs.SetInt("DernierStageFini", 1); 

        ActualiserBoutons();
    }

    public void ActualiserBoutons()
    {
        int dernierStageFini = PlayerPrefs.GetInt("DernierStageFini", 0);

        for (int i = 0; i < boutonsPersos.Count; i++)
        {
            int niveauRequis = i + 1;

            if (dernierStageFini >= niveauRequis)
            {
                boutonsPersos[i].interactable = true;
                if (imagesPersos.Count > i)
                {
                    imagesPersos[i].color = Color.white;
                }
            }
            else
            {
                
                boutonsPersos[i].interactable = false;
                if (imagesPersos.Count > i)
                {
                    
                    imagesPersos[i].color = Color.black;
                }
            }
        }
    }
}