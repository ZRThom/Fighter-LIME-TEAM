using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    [Tooltip("Vitesse de défilement en unités/pixels par seconde. Ajustez-la dans l'Inspecteur !")]
    public float scrollSpeed = 50f;

    [Tooltip("Position Y (coordonnées monde/Canvas) où le défilement doit s'arrêter.")]
    public float stopYPosition = 5000f; 

    private Transform objectTransform;
    private Vector3 startPosition;
    private bool isScrolling = true;

    void Awake()
    {
        objectTransform = GetComponent<Transform>();
        startPosition = objectTransform.position;
    }

    void OnEnable()
    {
        if (objectTransform != null)
        {
            objectTransform.position = startPosition;
            isScrolling = true;
        }
    }

    void Update()
    {
        if (!isScrolling) return;

        objectTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if (objectTransform.position.y > stopYPosition)
        {
            isScrolling = false;
        }
    }
}