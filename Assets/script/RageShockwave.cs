using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RageShockwave : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float duration = 0.5f;
    [Header("Scale")]
    [SerializeField] private Vector3 startScale = new Vector3(1f, 1f, 1f);
    [Header("Fade")]
    [SerializeField] private bool destroyAtEnd = true;

    private SpriteRenderer sr;
    private Vector3 targetScale;
    private float timer;
    private Color baseColor;
    private bool playing;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    public void Play(Vector3 worldPos, Vector3 endScale)
    {
        transform.position = worldPos;
        transform.localScale = startScale;

        targetScale = endScale;
        timer = 0f;
        playing = true;
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        baseColor = sr.color;
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
    }

    private void Update()
    {
        if (!playing) return;
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        float alpha = Mathf.Lerp(1f, 0f, t);
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

        if (t >= 1f)
        {
            playing = false;
            if (destroyAtEnd)
            {
                Destroy(gameObject);
            }
        }
    }
}
