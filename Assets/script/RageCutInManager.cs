using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RageCutInManager : MonoBehaviour
{
    public static RageCutInManager Instance;

    [Header("Root UI")]
    public GameObject root;
    public RectTransform backgroundRect;
    public RectTransform portraitRect;

    [Header("Animator")]
    public Animator backgroundAnimator;
    public Image portraitImage;

    [Header("Timing")]
    public float backgroundDuration = 3f;
    public float portraitDuration = 2f;
    public float portraitStartDelay = 0f;

    [Header("mvm")]
    public float offscreenX = 1500f;
    public float portraitY = 0f;
    public bool IsPlaying { get; private set; }
    private float previousTimeScale = 1f;

    [Header("Fade")]
    public CanvasGroup backgroundCanvasGroup;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        Instance = this;
        if (backgroundAnimator != null)
        {
            backgroundAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (backgroundCanvasGroup != null)
        {
            backgroundCanvasGroup.alpha = 0f;
        }

        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public bool PlayCutIn(int playerNumber, RuntimeAnimatorController bgController, Sprite portraitSprite)
    {
        if (IsPlaying) return false;
        StartCoroutine(PlaySequence(playerNumber, bgController, portraitSprite));
        return true;
    }

    IEnumerator PlaySequence(int playerNumber, RuntimeAnimatorController bgController, Sprite portraitSprite)
    {
        IsPlaying = true;
        
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (root != null)
        {
            root.SetActive(true);
        }

        if (backgroundAnimator != null && bgController != null)
        {
            backgroundAnimator.runtimeAnimatorController = bgController;
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = portraitSprite;
        }

        if (backgroundCanvasGroup != null)
        {
            backgroundCanvasGroup.alpha = 0f;
        }

        RestartAnimator(backgroundAnimator);

        bool fromLeft = playerNumber == 1;

        ApplyFlip(backgroundRect, fromLeft ? 1f : -1f);
        ApplyFlip(portraitRect, fromLeft ? 1f : -1f);

        if (portraitRect != null)
        {
            float startX = fromLeft ? -offscreenX : offscreenX;
            portraitRect.anchoredPosition = new Vector2(startX, portraitY);
        }

        //fade pour style
        Coroutine fadeInRoutine = null;
        if (backgroundCanvasGroup != null)
        {
            fadeInRoutine = StartCoroutine(FadeCanvasGroup(backgroundCanvasGroup, 0f, 1f, fadeDuration));
        }

        if (portraitStartDelay > 0f)
        {
            yield return WaitRealtime(portraitStartDelay);
        }

        Coroutine moveRoutine = null;
        if (portraitRect != null)
        {
            moveRoutine = StartCoroutine(MovePortrait(fromLeft));
        }

        float WaitBeforeFadeOut = Mathf.Max(0f, backgroundDuration - fadeDuration);
        yield return WaitRealtime(WaitBeforeFadeOut);

        // fade out
        if (backgroundCanvasGroup != null)
        {
            yield return FadeCanvasGroup(backgroundCanvasGroup, backgroundCanvasGroup.alpha, 0f, fadeDuration);
        }

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        if (root != null)
        {
            root.SetActive(false);
        }

        Time.timeScale = previousTimeScale;
        IsPlaying = false;
    }

    IEnumerator MovePortrait(bool fromLeft)
    {
        float startX = fromLeft ? -offscreenX : offscreenX;
        float endX = -startX;

        Vector2 start = new Vector2(startX, portraitY);
        Vector2 end = new Vector2(endX, portraitY);

        float timer = 0f;
        while (timer < portraitDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / portraitDuration);
            float path = EvaluateTravel(t);
            portraitRect.anchoredPosition = Vector2.LerpUnclamped(start, end, path);
            yield return null;
        }
        portraitRect.anchoredPosition = end;
    }

    float EvaluateTravel(float t)
    {
        // path (anim : rapide, puis lent, puis rapide (style street))
        // 2eme try, arrive lent, puis repart vitesse inverse (lineaire)
        if (t < 0.5f)
        {
            float p = t / 0.5f;
            return Mathf.Lerp(0f, 0.5f, EaseOutQuad(p));
        }
        else
        {
            float p = (t - 0.5f) / 0.5f;
            // calc zone de c** (centre)
            return Mathf.Lerp(0.5f, 1f, EaseInQuad(p));
        }
    }

    float EaseOutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    float EaseInQuad(float x)
    {
        return x * x;
    }

    void RestartAnimator(Animator anim)
    {
        if (anim == null) return;
        anim.Rebind();
        anim.Update(0f);
        anim.Play(0, 0, 0f);
    }

    void ApplyFlip(RectTransform rect, float xSign)
    {
        if (rect == null) return;
        Vector3 s = rect.localScale;
        s.x = Mathf.Abs(s.x) * xSign;
        rect.localScale = s;
    }

    IEnumerator WaitRealtime(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null)
        {
            yield break;
        }
        float timer = 0f;
        cg.alpha = from;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        cg.alpha = to;
    }
}
