using UnityEngine;

public class RealMenuManager : MonoBehaviour
{
    public static bool openStoryPanelOnLoad = false;
    public static bool open1v1PanelOnLoad = false;

    [Header("panel de base")]
    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelSettings;
    public GameObject panelSettingsCommand;
    public GameObject panelCredits;
    public GameObject panelProfil;

    public GameObject panelRaph;
    public AudioClip musicRaph;


    [Header("panel sous panelPlay")]
    public GameObject panelPlay1v1;
    public GameObject panelPlayTraining;
    public GameObject panelPlayStory;

    [Header("panel sous panelProfil")]
    public GameObject panelPerso1;
    public GameObject panelPerso2;
    public GameObject panelPerso3;
    public GameObject panelPerso4;
    public GameObject panelPerso5;
    public AudioClip musicPerso1;
    public AudioClip musicPerso2;
    public AudioClip musicPerso3;
    public AudioClip musicPerso4;
    public AudioClip musicPersoKiffeur;

    void Start()
    {
        OpenMainMenu();
        
        if (open1v1PanelOnLoad)
        {
            open1v1PanelOnLoad = false;
            PanelPlay1v1();
        }

        if (openStoryPanelOnLoad)
        {
            openStoryPanelOnLoad = false;
            PanelPlayStory();
        }
    }

    void HideAllPanels()
    {
        panelMenu.SetActive(false);
        panelPlay.SetActive(false);
        panelSettings.SetActive(false);
        panelCredits.SetActive(false);
        panelProfil.SetActive(false);
        panelSettingsCommand.SetActive(false);

        panelRaph.SetActive(false);
        
        panelPerso1.SetActive(false);
        panelPerso2.SetActive(false);
        panelPerso3.SetActive(false);
        panelPerso4.SetActive(false);
        panelPerso5.SetActive(false);

        panelPlay1v1.SetActive(false);
        panelPlayTraining.SetActive(false);
        panelPlayStory.SetActive(false);
    }

    void OpenMainMenu()
    {
        HideAllPanels();
        panelMenu.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }

    // play
    public void PanelPlay()
    {
        HideAllPanels();
        panelPlay.SetActive(true);
    }

    public void PanelPlayBack()
    {
        OpenMainMenu();
    }

    // settings
    public void PanelSettings()
    {
        HideAllPanels();
        panelSettings.SetActive(true);
    }

    public void PanelSettingsBack()
    {
        OpenMainMenu();
    }

    // credit
    public void PanelCredits()
    {
        HideAllPanels();
        panelCredits.SetActive(true);
    }

    public void PanelCreditsBack()
    {
        OpenMainMenu();
    }

    // Profil
    public void PanelProfil()
    {
        HideAllPanels();
        panelProfil.SetActive(true);
    }

    public void PanelProfilBack()
    {
        OpenMainMenu();
    }

    public void PanelRaph()
    {
        HideAllPanels();
        panelRaph.SetActive(true);
        
        if (AudioManager.instance != null && musicRaph != null)
        {
            AudioManager.instance.PlayMusic(musicRaph);
        }
    }

    public void PanelRaphBack()
    {
        OpenMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }

    // profil perso 1
    public void PanelProfilPerso1()
    {
        HideAllPanels();
        panelPerso1.SetActive(true);
        
        if (AudioManager.instance != null && musicPerso1 != null)
        {
            AudioManager.instance.PlayMusic(musicPerso1);
        }
    }

    public void PanelProfilPerso1Back()
    {
        HideAllPanels();
        panelProfil.SetActive(true);
    }

    // profil perso 2
    public void PanelProfilPerso2()
    {
        HideAllPanels();
        panelPerso2.SetActive(true);
        
        if (AudioManager.instance != null && musicPerso2 != null)
        {
            AudioManager.instance.PlayMusic(musicPerso2);
        }
    }

    public void PanelProfilPerso2Back()
    {
        HideAllPanels();
        panelProfil.SetActive(true);
    }

    // profil perso 3
    public void PanelProfilPerso3()
    {
        HideAllPanels();
        panelPerso3.SetActive(true);
        
        if (AudioManager.instance != null && musicPerso3 != null)
        {
            AudioManager.instance.PlayMusic(musicPerso3);
        }
    }

    public void PanelProfilPerso3Back()
    {
        HideAllPanels();
        panelProfil.SetActive(true);
    }

    // profil perso 4
    public void PanelProfilPerso4()
    {
        HideAllPanels();
        panelPerso4.SetActive(true);
        
        if (AudioManager.instance != null && musicPerso4 != null)
        {
            AudioManager.instance.PlayMusic(musicPerso4);
        }
    }

    public void PanelProfilPerso4Back()
    {
        HideAllPanels();
        panelProfil.SetActive(true);
    }

    // profil perso 5
    public void PanelProfilPerso5()
    {
        HideAllPanels();
        panelPerso5.SetActive(true);
        
        if (AudioManager.instance != null && musicPersoKiffeur != null)
        {
            AudioManager.instance.PlayMusic(musicPersoKiffeur);
        }
    }

    public void PanelProfilPerso5Back()
    {
        HideAllPanels();
        panelProfil.SetActive(true);
    }

    // settings
    public void PanelSettingsCommand()
    {
        HideAllPanels();
        panelSettings.SetActive(true);
        panelSettingsCommand.SetActive(true);
    }

    // play training
    public void PanelPlayTraining()
    {
        HideAllPanels();
        panelPlayTraining.SetActive(true);
    }

    public void PanelPlayTrainingBack()
    {
        HideAllPanels();
        panelPlay.SetActive(true);
    }

    // play 1v1
    public void PanelPlay1v1()
    {
        HideAllPanels();
        panelPlay1v1.SetActive(true);
    }

    public void PanelPlay1v1Back()
    {
        HideAllPanels();
        panelPlay.SetActive(true);
    }

    // play Story
    public void PanelPlayStory()
    {
        HideAllPanels();
        panelPlayStory.SetActive(true);
    }

    public void PanelPlayStoryBack()
    {
        HideAllPanels();
        panelPlay.SetActive(true);
    }
}
