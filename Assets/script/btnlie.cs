using UnityEngine;
using UnityEngine.SceneManagement; // ESSENTIEL pour changer de scène

public class SceneLoader : MonoBehaviour
{
    // ----------------------------------------------------------------------
    // FONCTION POUR CHARGER UNE NOUVELLE SCÈNE (À LIER AU BOUTON START)
    // ----------------------------------------------------------------------
    
    /// <summary>
    /// Charge la scène de jeu.
    /// Pour que cela fonctionne, les scènes doivent être ajoutées dans File > Build Settings.
    /// </summary>
    /// <param name="sceneIndex">L'index de la scène dans les Build Settings (par exemple, 1 pour la scène de jeu).</param>
    public void LoadGameScene(int sceneIndex)
    {
        // Change la scène en utilisant l'index fourni.
        SceneManager.LoadScene(sceneIndex);
        
        // --- Méthode alternative si vous préférez utiliser le NOM de la scène : ---
        // SceneManager.LoadScene("NomDeVotreSceneDeJeu");
     }
 }
