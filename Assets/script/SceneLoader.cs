using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Bibliothèque de Sons")]
    [SerializeField] private AudioClip submitSound; // Ton son 'Validé'
    [SerializeField] private AudioClip backSound;   // Ton son 'Retour'

    // ---------------------------------------------------------
    // PARTIE 1 : LES SONS (À ajouter en PREMIER dans l'inspecteur)
    // ---------------------------------------------------------

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

    // ---------------------------------------------------------
    // PARTIE 2 : LA NAVIGATION (À ajouter en DEUXIÈME)
    // ---------------------------------------------------------

    // J'ai gardé le nom exact que tu voulais.
    // Attention : Elle ne joue plus de son toute seule maintenant !
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
}