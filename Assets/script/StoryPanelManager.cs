using UnityEngine;

public class StoryPanelManager : MonoBehaviour
{
    [Header("Boss Panels")]
    public GameObject Boss1;
    public GameObject Boss2;
    public GameObject Boss3;
    public GameObject Boss4;
    
    [Header("Boss Music")]
    public AudioClip musicBoss1;
    public AudioClip musicBoss2;
    public AudioClip musicBoss3;
    public AudioClip musicBoss4;

    [Header("Versus Panels")]
    public GameObject VersusBOSS1;
    public GameObject VersusBOSSS2;
    public GameObject VersusBOSSS3;
    public GameObject VersusBOSSS4;

    [Header("W Panels")]
    public GameObject w1;
    public GameObject w2;
    public GameObject w3;
    public GameObject w4;

    [Header("L Panels")]
    public GameObject l1;
    public GameObject l2;
    public GameObject l3;
    public GameObject l4;

    [Header("Lore Panel")]
    public GameObject LorePanel;
    
    void Start()
    {
        
        OpenVersusBoss1();
    }

    public void HideAllPanels()
    {
        if (Boss1 != null) Boss1.SetActive(false);
        if (Boss2 != null) Boss2.SetActive(false);
        if (Boss3 != null) Boss3.SetActive(false);
        if (Boss4 != null) Boss4.SetActive(false);

        if (VersusBOSS1 != null) VersusBOSS1.SetActive(false);
        if (VersusBOSSS2 != null) VersusBOSSS2.SetActive(false);
        if (VersusBOSSS3 != null) VersusBOSSS3.SetActive(false);
        if (VersusBOSSS4 != null) VersusBOSSS4.SetActive(false);

        if (w1 != null) w1.SetActive(false);
        if (w2 != null) w2.SetActive(false);
        if (w3 != null) w3.SetActive(false);
        if (w4 != null) w4.SetActive(false);

        if (l1 != null) l1.SetActive(false);
        if (l2 != null) l2.SetActive(false);
        if (l3 != null) l3.SetActive(false);
        if (l4 != null) l4.SetActive(false);

        if (LorePanel != null) LorePanel.SetActive(false);
    }

    // --- Lore ---
    public void OpenLorePanel()
    {
        HideAllPanels();
        if (LorePanel != null) LorePanel.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }

    // --- Boss Panels ---
    public void OpenBoss1()
    {
        HideAllPanels();
        if (Boss1 != null) Boss1.SetActive(true);
        
        if (AudioManager.instance != null && musicBoss1 != null)
        {
            AudioManager.instance.PlayMusic(musicBoss1);
        }
    }
    public void OpenBoss2()
    {
        HideAllPanels();
        if (Boss2 != null) Boss2.SetActive(true);
        
        if (AudioManager.instance != null && musicBoss2 != null)
        {
            AudioManager.instance.PlayMusic(musicBoss2);
        }
    }
    public void OpenBoss3()
    {
        HideAllPanels();
        if (Boss3 != null) Boss3.SetActive(true);
        
        if (AudioManager.instance != null && musicBoss3 != null)
        {
            AudioManager.instance.PlayMusic(musicBoss3);
        }
    }
    public void OpenBoss4()
    {
        HideAllPanels();
        if (Boss4 != null) Boss4.SetActive(true);
        
        if (AudioManager.instance != null && musicBoss4 != null)
        {
            AudioManager.instance.PlayMusic(musicBoss4);
        }
    }

    // --- Versus Panels ---
    public void OpenVersusBoss1()
    {
        HideAllPanels();
        if (VersusBOSS1 != null) VersusBOSS1.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenVersusBoss2()
    {
        HideAllPanels();
        if (VersusBOSSS2 != null) VersusBOSSS2.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenVersusBoss3()
    {
        HideAllPanels();
        if (VersusBOSSS3 != null) VersusBOSSS3.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenVersusBoss4()
    {
        HideAllPanels();
        if (VersusBOSSS4 != null) VersusBOSSS4.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }

    // --- W Panels (Victoire) ---
    public void OpenW1()
    {
        HideAllPanels();
        if (w1 != null) w1.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenW2()
    {
        HideAllPanels();
        if (w2 != null) w2.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenW3()
    {
        HideAllPanels();
        if (w3 != null) w3.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenW4()
    {
        HideAllPanels();
        if (w4 != null) w4.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }

    // --- L Panels (Défaite) ---
    public void OpenL1()
    {
        HideAllPanels();
        if (l1 != null) l1.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenL2()
    {
        HideAllPanels();
        if (l2 != null) l2.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenL3()
    {
        HideAllPanels();
        if (l3 != null) l3.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
    public void OpenL4()
    {
        HideAllPanels();
        if (l4 != null) l4.SetActive(true);
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDefaultMusic();
        }
    }
}
