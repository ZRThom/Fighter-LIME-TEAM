using UnityEngine;
using System.Collections;

public class CameraFollow2d : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform block;
    [SerializeField] float FollowSpeed = 2f;
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] float zoomMultiplier = 1f;
    [SerializeField] float minZoom = 3f;
    [SerializeField] float maxZoom = 11f;   

    Camera cam;

    void Start()
    {
    cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // search mid + distance
        var midpoint = (player.position + block.position) * 0.5f;
        var dist = Vector2.Distance(player.position, block.position);

        // camera move
        var targetPos = new Vector3(midpoint.x, midpoint.y, cam.transform.position.z);
        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            targetPos,
            FollowSpeed * Time.deltaTime
        );

        // zoom adaptation
        var targetSize = Mathf.Clamp(dist * zoomMultiplier , minZoom, maxZoom);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );
    }
}