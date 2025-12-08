using UnityEngine;
using UnityEngine.SceneManagement; // Importez SceneManager !

public class SceneLoader : MonoBehaviour
{
    // Fonction utilisée par les boutons 'play', 'ABOUT', 'SETUP'
    public void LoadSceneByName(string sceneName)
    {
        // Assurez-vous que la scène est dans les Build Settings
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Nom de scène non spécifié !");
        }
    }

    // Fonction utilisée par le bouton 'EXIT'
    public void QuitGame()
    {
        // Quitter l'application (fonctionne dans l'application compilée)
        Application.Quit();

        // Arrêter le mode Play dans l'éditeur Unity (pour les tests)
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}