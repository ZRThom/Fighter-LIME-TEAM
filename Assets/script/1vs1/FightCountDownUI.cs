using UnityEngine;
using TMPro;
using System.Collections;

public class FightCountDownUI : MonoBehaviour
{
    [Header("tmp")]
    [SerializeField] private TMP_Text textCountDown;

    [Header("time")]
    [SerializeField] private float CdDuration = 1f;

    [Header("text")]
    [SerializeField] private string text1 = "1";
    [SerializeField] private string text2 = "2";
    [SerializeField] private string text3 = "3";
    [SerializeField] private string textFight = "FIGHT";

    [Header("Scale")]
    [SerializeField] private float nrmlScale = 1f;

    public float TotalDuration => CdDuration * 4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (textCountDown != null) textCountDown.gameObject.SetActive(false);
    }

    public void PlayCountdown()
    {
        if (textCountDown == null)
        {
            Debug.LogWarning("FightCountDownUI has no tmptext");
            return;
        }

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
        Debug.Log("countdown throw");
        textCountDown.text = textToShow;
        textCountDown.transform.localScale = Vector3.one * nrmlScale;

        Color c = textCountDown.color;
        c.a = 1f;
        textCountDown.color = c;
        float t = 0f;
        while (t < CdDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / CdDuration);
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
        textCountDown.transform.localScale = Vector3.one * nrmlScale;

        Color c = textCountDown.color;
        c.a = 1f;
        textCountDown.color = c;

        yield return new WaitForSeconds(CdDuration);
    }
}
