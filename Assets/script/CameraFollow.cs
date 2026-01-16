using UnityEngine;

public class CameraFollow2d : MonoBehaviour
{
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

        // Position cible de la caméra
        var targetPos = new Vector3(midpoint.x + offset.x, midpoint.y, cam.transform.position.z);

        // Déplacement fluide de la caméra
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
