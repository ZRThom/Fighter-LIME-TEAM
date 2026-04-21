using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonTraining : MonoBehaviour
{
    public string targetSceneBase;
    public string loadingScene = "SceneLoading";

    public void Play()
    {
        // On vérifie uniquement si le Joueur 1 est sélectionné
        if (GameManagerSelect.Instance.firstSelectedPrefab == null)
        {
            Debug.Log("Player 1 not selected for training");
            return;
        }
        
        SceneCompactLoader.sceneToLoad = targetSceneBase;
        SceneManager.LoadScene(loadingScene);
    }
}