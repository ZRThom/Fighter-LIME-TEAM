using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string targetScene;
    public void GoToNextScene()
    {
        Time.timeScale = 1f; 
        SceneCompactLoader.sceneToLoad = targetScene;
        SceneManager.LoadScene("SceneLoading");
    }

    public void GoToNextSceneBase()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(targetScene);
    }

}
