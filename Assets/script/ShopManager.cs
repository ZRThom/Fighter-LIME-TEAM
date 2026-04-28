using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Liste des Boutons de Persos (dans l'ordre 1-4)")]
    public List<Button> boutonsPersos;

    [Header("Liste des Images de Persos (sur les boutons)")]
    public List<Image> imagesPersos;

    void Start()
    {
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
                // DÉBLOQUÉ
                boutonsPersos[i].interactable = true;
                if (imagesPersos.Count > i)
                {
                    imagesPersos[i].color = Color.white; // Couleur normale
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

    public void DebloquerStage(int numeroDuStage)
    {
        PlayerPrefs.SetInt("DernierStageFini", numeroDuStage);
        PlayerPrefs.Save();
        ActualiserBoutons(); // Rafraîchit direct le shop
    }
}