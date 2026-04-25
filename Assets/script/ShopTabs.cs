using UnityEngine;

public class ShopTabs : MonoBehaviour
{
    public GameObject pagePersos;
    public GameObject pageCosmetiques;

    public void OuvrirOngletPersos() 
    {
        pagePersos.SetActive(true);
        pageCosmetiques.SetActive(false);
    }

    public void OuvrirOngletCosmetiques() 
    {
        pagePersos.SetActive(false);
        pageCosmetiques.SetActive(true);
    }
}