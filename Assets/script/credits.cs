using UnityEngine;
// using TMPro; // Inutile pour ce script, mais nécessaire pour les composants TextMeshPro

public class CreditScroller : MonoBehaviour
{
    [Tooltip("Vitesse de défilement en unités/pixels par seconde. Ajustez-la dans l'Inspecteur !")]
    public float scrollSpeed = 50f;

    [Tooltip("Position Y (coordonnées monde/Canvas) où le défilement doit s'arrêter.")]
    public float stopYPosition = 1500f; 

    private Transform objectTransform;

    void Start()
    {
        // Récupère le transform de l'objet (votre CreditScrollerContainer) au démarrage.
        objectTransform = GetComponent<Transform>();
    }

    void Update()
    {
        // Déplace l'objet (le conteneur) vers le haut sur l'axe Y.
        // Utilise Time.deltaTime pour garantir un défilement constant, quelle que soit la fréquence d'images.
        objectTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        // Vérifie si le conteneur a dépassé la position de fin
        if (objectTransform.position.y > stopYPosition)
        {
            // Arrête le défilement en désactivant ce composant (le script lui-même)
            enabled = false;
            
            // Si vous préférez détruire l'objet complètement :
            // Destroy(gameObject);
        }
    }
}