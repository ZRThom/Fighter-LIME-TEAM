using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonAll : MonoBehaviour
{
    public string targetSceneBase;
    public void Play()
    {
    if (GameManager.Instance.player1Prefab == null || GameManager.Instance.player2Prefab == null)
        {
            Debug.Log("Players not selected");
            return;
        }
        SceneManager.LoadScene(targetSceneBase);
    }
}
