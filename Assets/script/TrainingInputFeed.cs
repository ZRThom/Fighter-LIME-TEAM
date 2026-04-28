using UnityEngine;
using System.Collections.Generic;

public class TrainingInputFeed : MonoBehaviour
{
    [Header("visual")]
    [SerializeField] private RectTransform view;
    [SerializeField] private RectTransform rowsRoot;
    [SerializeField] private TrainingInputFeedRow rowPrefab;

    [Header("layout")]
    [SerializeField] private float rowSpacing = 42f;
    [SerializeField] private float slideDuration = 0.05f; 
    
    [Header("frames")]
    [SerializeField] private int maxFrameDisplay = 200;
    [SerializeField] private bool showFrameOnFirstInput = false; 

    private readonly List<TrainingInputFeedRow> rows = new();
    private int currentFrame;
    private int lastInputFrame = -1;

    private void Awake()
    {
        if (!view) view = GetComponent<RectTransform>();
        if (rowsRoot) rowsRoot = view;
    }

    void Update()
    {
        currentFrame++;
    }

    public void RegisterInput(Sprite inputSprite)
    {
        if (!inputSprite || !rowPrefab) return;
        
        int frameDelta = 0;
        bool hasPreviousInput = lastInputFrame >= 0;

        if (hasPreviousInput) frameDelta = Mathf.Clamp(currentFrame - lastInputFrame, 0, maxFrameDisplay);
        lastInputFrame = currentFrame;

        TrainingInputFeedRow row = Instantiate(rowPrefab, rowsRoot);
        row.Init(inputSprite, frameDelta, hasPreviousInput || showFrameOnFirstInput);
        row.Rect.anchoredPosition = new Vector2(row.Rect.anchoredPosition.x, 0f);
        rows.Insert(0, row);

        RepositionRows();
        DestroyRowsOutsideView();
    }

    private void RepositionRows()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            float targetY = i * rowSpacing;
            rows[i].MoveToY(targetY, slideDuration);
        }
    }

    private void DestroyRowsOutsideView()
    {
        float KillY = view.rect.height;
        for (int i = rows.Count - 1; i >= 0; i--)
        {
            TrainingInputFeedRow row = rows[i];
            if (!row)
            {
                rows.RemoveAt(i);
                continue;
            }

            // bug, mais patch : check y = 0, quand la ligne arrive sur zone visible, destroy
            if (row.TargetY >= KillY)
            {
                Destroy(row.gameObject);
                rows.RemoveAt(i);
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i]) Destroy(rows[i].gameObject);
        }

        rows.Clear();
        lastInputFrame = -1;
    }
}
