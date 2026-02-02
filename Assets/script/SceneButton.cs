using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string targetScene;
    public void GoToNextScene()
    {
        SceneCompactLoader.sceneToLoad = targetScene;
        SceneManager.LoadScene("SceneLoading");
    }
}
