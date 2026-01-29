using UnityEngine;

public class CameraFollow2d : MonoBehaviour
{
    [SerializeField] float xMin = -10f;
    [SerializeField] float xMax = 10f;
    [SerializeField] float yMin = -5f;
    [SerializeField] float yMax = 5f;

    [SerializeField] Transform player;
    [SerializeField] Transform block;
    [SerializeField] float followSpeed = 2f;
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] float zoomMultiplier = 1f;
    [SerializeField] float minZoom = 3f;
    [SerializeField] float maxZoom = 11f;
    [SerializeField] Vector2 offset = new Vector2(0, 2f); // Décalage vertical

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // Calcul du midpoint entre joueur et bloc
        var midpoint = (player.position + block.position) * 0.5f;

        // Ajout du décalage vertical pour voir plus haut
        midpoint.y += offset.y;

        // Position cible avec offset
        var targetPos = new Vector3(midpoint.x + offset.x, midpoint.y, cam.transform.position.z);

        // 🔹 Limiter la caméra
        targetPos.x = Mathf.Clamp(targetPos.x, xMin, xMax);
        targetPos.y = Mathf.Clamp(targetPos.y, yMin, yMax);

        // Déplacement fluide
        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );


        // Calcul de la distance pour zoomer
        var dist = Vector2.Distance(player.position, block.position);
        var targetSize = Mathf.Clamp(dist * zoomMultiplier, minZoom, maxZoom);

        // Zoom fluide
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );
    }
}
