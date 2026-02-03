using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    [Tooltip("Vitesse de défilement en unités/pixels par seconde. Ajustez-la dans l'Inspecteur !")]
    public float scrollSpeed = 50f;

    [Tooltip("Position Y (coordonnées monde/Canvas) où le défilement doit s'arrêter.")]
    public float stopYPosition = 5000f; 

    private Transform objectTransform;

    void Start()
    {
        objectTransform = GetComponent<Transform>();
    }

    void Update()
    {
        objectTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if (objectTransform.position.y > stopYPosition)
        {
            enabled = false;
            
            // Destroy(gameObject);
        }
    }
}