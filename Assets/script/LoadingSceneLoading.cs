using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingSceneLoading : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public float waitTime = 2f;
    void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        yield return StartCoroutine(Fade(1, 0));
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(Fade(0, 1));
        if (!string.IsNullOrEmpty(SceneCompactLoader.sceneToLoad)) SceneManager.LoadScene(SceneCompactLoader.sceneToLoad);
        else Debug.LogError("Empty path : SceneCompactLoader.sceneToLoad");
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
    }
}
