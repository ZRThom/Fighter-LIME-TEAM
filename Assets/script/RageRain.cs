using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RageRain : MonoBehaviour
{
    [Header("Place")]
    public float extraX = 2f;
    public float extraY = 2f;

    [Header("Speed")]
    public float horizontalSpeed = 10f;
    public float verticalspeed = -25f;

    [Header("Render")]
    public string sortingLayerName = "FX";
    public int sortingOrder = 100;
    private ParticleSystem ps;
    private ParticleSystemRenderer psr;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();

        if (psr != null)
        {
            psr.sortingLayerName = sortingLayerName;
            psr.sortingOrder = sortingOrder;
        }
    }

    public void Play(bool fromRight)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Destroy(gameObject, 2f);
            return;
        }

        float zDist = Mathf.Abs(cam.transform.position.z - transform.position.z);

        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0f, 1f, zDist));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, zDist));

        transform.position = fromRight ? new Vector3(topRight.x + extraX, topRight.y + extraY, 0f) : new Vector3(topLeft.x + extraX, topLeft.y + extraY, 0f);

        var vol = ps.velocityOverLifetime;
        vol.enabled = true;
        vol.space = ParticleSystemSimulationSpace.World;
        vol.x = new ParticleSystem.MinMaxCurve(fromRight ? -horizontalSpeed : horizontalSpeed);
        vol.y = new ParticleSystem.MinMaxCurve(verticalspeed);
        vol.z = new ParticleSystem.MinMaxCurve(0f);

        ps.Play(true);
    }
}
