using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingInputFeedRow : MonoBehaviour
{
    [Header("ref")]
    [SerializeField] private Image inputImg;
    [SerializeField] private TMP_Text frameTxt;

    public RectTransform Rect { get; private set; }
    public float TargetY { get; private set; }

    private float startY;
    private float elapsed;
    private float duration;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        if (!inputImg) inputImg = GetComponentInChildren<Image>();
        if (!frameTxt) frameTxt = GetComponentInChildren<TMP_Text>();
    }

    public void Init(Sprite inputSprite, int frameDelta, bool showFrame)
    {
        if (inputImg) inputImg.sprite = inputSprite;
        if (frameTxt) frameTxt.text = showFrame ? frameDelta.ToString() : "";
    }
    
    public void MoveToY(float y, float moveDuration)
    {
        TargetY = y;
        startY = Rect.anchoredPosition.y;
        elapsed = 0f;
        duration = Mathf.Max(0f, moveDuration);
        if (duration <= 0f) SetY(TargetY);
    }

    private void Update()
    {
        if (duration <= 0f) return;
        elapsed += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        float eased = Mathf.SmoothStep(0f, 1f, t);
        float y = Mathf.Lerp(startY, TargetY, eased);
        SetY(y);
        if (t >= 1f) duration = 0f;
    }

    private void SetY(float y)
    {
        Vector2 pos = Rect.anchoredPosition;
        pos.y = y;
        Rect.anchoredPosition = pos;
    }
}
