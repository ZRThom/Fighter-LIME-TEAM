using UnityEngine;
using TMPro;
using System.Collections;

public class StoryCountDownUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TMP_Text textCountDown;

    [Header("Settings")]
    [SerializeField] private float cdDuration = 1f;
    [SerializeField] private float normalScale = 1f;

    [Header("Texts")]
    [SerializeField] private string text1 = "1";
    [SerializeField] private string text2 = "2";
    [SerializeField] private string text3 = "3";
    [SerializeField] private string textFight = "FIGHT";

    public float TotalDuration => cdDuration * 4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (textCountDown != null) textCountDown.gameObject.SetActive(false);
    }

    public void PlayCountdown()
    {
        if (textCountDown == null) return;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        textCountDown.gameObject.SetActive(true);

        yield return StartCoroutine(ShowAndFade(text3));
        yield return StartCoroutine(ShowAndFade(text2));
        yield return StartCoroutine(ShowAndFade(text1));
        yield return StartCoroutine(ShowFight());

        textCountDown.gameObject.SetActive(false);
        currentRoutine = null;
    }

    private IEnumerator ShowAndFade(string textToShow)
    {
        textCountDown.text = textToShow;
        textCountDown.transform.localScale = Vector3.one * normalScale;

        Color c = textCountDown.color;
        c.a = 1f;
        textCountDown.color = c;
        
        float t = 0f;
        while (t < cdDuration)
        {
            t += Time.unscaledDeltaTime; 
            float alpha = Mathf.Lerp(1f, 0f, t / cdDuration);
            c.a = alpha;
            textCountDown.color = c;
            yield return null;
        }
        c.a = 0f;
        textCountDown.color = c;
    }

    private IEnumerator ShowFight()
    {
        textCountDown.text = textFight;
        textCountDown.transform.localScale = Vector3.one * normalScale;

        Color c = textCountDown.color;
        c.a = 1f;
        textCountDown.color = c;

        yield return new WaitForSecondsRealtime(cdDuration);
    }
}