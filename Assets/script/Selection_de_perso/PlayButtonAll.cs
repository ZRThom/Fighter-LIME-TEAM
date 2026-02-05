using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonAll : MonoBehaviour
{
    public string targetSceneBase;
    public string loadingScene = "SceneLoading";
    public void Play()
    {
    if (GameManager.Instance.firstSelectedPrefab == null || GameManager.Instance.secondSelectedPrefab == null)
        {
            Debug.Log("Players not selected");
            return;
        }
        SceneCompactLoader.sceneToLoad = targetSceneBase;
        SceneManager.LoadScene(loadingScene);
    }
}
