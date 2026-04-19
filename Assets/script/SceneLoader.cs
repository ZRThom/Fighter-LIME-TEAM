using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Bibliothèque de Sons")]
    [SerializeField] private AudioClip submitSound; // Ton son 'Validé'
    [SerializeField] private AudioClip backSound;   // Ton son 'Retour'

    public void PlaySubmitSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(submitSound);
    }

    public void PlayBackSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(backSound);
    }
    public void LoadSceneByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Erreur : Pas de nom de scène !");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void DebugTest()
    {
        Debug.Log("Scene debug");
    }

    public void ReturnToMainMenuStory()
    {
        
        Time.timeScale = 1f;
        
        RealMenuManager.openStoryPanelOnLoad = true;
        LoadSceneByName("MainMenu");
    }
}
