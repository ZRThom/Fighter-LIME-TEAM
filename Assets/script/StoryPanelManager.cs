using UnityEngine;

public class StoryPanelManager : MonoBehaviour
{
    [Header("Boss Panels")]
    public GameObject Boss1;
    public GameObject Boss2;
    public GameObject Boss3;
    public GameObject Boss4;
    
    [Header("Versus Panels")]
    public GameObject VersusBOSS1;
    public GameObject VersusBOSSS2;
    public GameObject VersusBOSSS3;
    public GameObject VersusBOSSS4;

    [Header("WL Panels")]
    public GameObject wl1;
    public GameObject wl2;
    public GameObject wl3;
    public GameObject wl4;

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

        if (wl1 != null) wl1.SetActive(false);
        if (wl2 != null) wl2.SetActive(false);
        if (wl3 != null) wl3.SetActive(false);
        if (wl4 != null) wl4.SetActive(false);

        if (LorePanel != null) LorePanel.SetActive(false);
    }

    // --- Lore ---
    public void OpenLorePanel()
    {
        HideAllPanels();
        if (LorePanel != null) LorePanel.SetActive(true);
    }

    // --- Boss Panels ---
    public void OpenBoss1()
    {
        HideAllPanels();
        if (Boss1 != null) Boss1.SetActive(true);
    }
    public void OpenBoss2()
    {
        HideAllPanels();
        if (Boss2 != null) Boss2.SetActive(true);
    }
    public void OpenBoss3()
    {
        HideAllPanels();
        if (Boss3 != null) Boss3.SetActive(true);
    }
    public void OpenBoss4()
    {
        HideAllPanels();
        if (Boss4 != null) Boss4.SetActive(true);
    }

    // --- Versus Panels ---
    public void OpenVersusBoss1()
    {
        HideAllPanels();
        if (VersusBOSS1 != null) VersusBOSS1.SetActive(true);
    }
    public void OpenVersusBoss2()
    {
        HideAllPanels();
        if (VersusBOSSS2 != null) VersusBOSSS2.SetActive(true);
    }
    public void OpenVersusBoss3()
    {
        HideAllPanels();
        if (VersusBOSSS3 != null) VersusBOSSS3.SetActive(true);
    }
    public void OpenVersusBoss4()
    {
        HideAllPanels();
        if (VersusBOSSS4 != null) VersusBOSSS4.SetActive(true);
    }

    // --- WL Panels ---
    public void OpenWL1()
    {
        HideAllPanels();
        if (wl1 != null) wl1.SetActive(true);
    }
    public void OpenWL2()
    {
        HideAllPanels();
        if (wl2 != null) wl2.SetActive(true);
    }
    public void OpenWL3()
    {
        HideAllPanels();
        if (wl3 != null) wl3.SetActive(true);
    }
    public void OpenWL4()
    {
        HideAllPanels();
        if (wl4 != null) wl4.SetActive(true);
    }
}
