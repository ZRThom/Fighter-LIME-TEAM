using UnityEngine;

public class CameraFollow2d : MonoBehaviour
{
    [SerializeField] float xMin = -10f;
    [SerializeField] float xMax = 10f;
    [SerializeField] float yMin = -5f;
    [SerializeField] float yMax = 5f;

    // Passés en public pour que le GameManager puisse les assigner
    public Transform player;
    public Transform player2;
    
    [SerializeField] float followSpeed = 2f;
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] float zoomMultiplier = 1f;
    [SerializeField] float minZoom = 3f;
    [SerializeField] float maxZoom = 11f;
    [SerializeField] Vector2 offset = new Vector2(0, 2f);

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // 🔹 SÉCURITÉ : Si les joueurs ne sont pas encore apparus, on ne fait rien !
        if (player == null || player2 == null) return;

        // Calcul du midpoint entre les deux joueurs 
        var midpoint = (player.position + player2.position) * 0.5f;

        // Ajout du décalage vertical pour voir plus haut
        midpoint.y += offset.y;

        // Position cible avec offset
        var targetPos = new Vector3(midpoint.x + offset.x, midpoint.y, cam.transform.position.z);

        // Limiter la caméra
        targetPos.x = Mathf.Clamp(targetPos.x, xMin, xMax);
        targetPos.y = Mathf.Clamp(targetPos.y, yMin, yMax);

        // Déplacement fluide
        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

        // Calcul de la distance pour zoomer
        var dist = Vector2.Distance(player.position, player2.position);
        var targetSize = Mathf.Clamp(dist * zoomMultiplier, minZoom, maxZoom);

        // Zoom fluide
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );
    }
}