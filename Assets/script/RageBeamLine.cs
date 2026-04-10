using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RageBeamLine : MonoBehaviour
{
    [Header("LifeTime")]
    public float duration = 0.5f;
    public float extraOffScreen = 2f;

    

    [Header("Shape")]
    public int segmentCount = 10;
    public float jitterAmount = 0.5f;
    public float jitterRefreshRate = 30f;
    
    [Header("Texture")]
    public float textureScrollSpeed = 2f;

    private LineRenderer lr;
    private float lifeTimer;
    private float jitterTimer;

    private Vector3 startPos;
    private Vector3 endPos;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
       //lr.positionCount = 2;
    }

    public void Play(bool goRight, float yWorld)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Destroy(gameObject, duration);
            return;
        }

        float zDist = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, zDist));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, zDist));

        //startPos = goRight ? new Vector3(left.x - extraOffScreen, yWorld, transform.position.z) : new Vector3(right.x + extraOffScreen, yWorld, transform.position.z);
        //endPos = goRight ? new Vector3(right.x + extraOffScreen, yWorld, transform.position.z) : new Vector3(left.x - extraOffScreen, yWorld, transform.position.z);
    
        startPos = goRight ? new Vector3(left.x - extraOffScreen, yWorld, 0f) : new Vector3(right.x + extraOffScreen, yWorld, 0f);
        endPos = goRight ? new Vector3(right.x + extraOffScreen, yWorld, 0f) : new Vector3(left.x - extraOffScreen, yWorld, 0f);
        
        lifeTimer = 0f;
        jitterTimer = 0f;
        RebuildBeam();
        
        //BuildJaggedBeam(startPos, endPos);

        // pos du joueur 1 avec beam, vers la direction du joueur 2
        //lr.SetPosition(1, startPos);
        //lr.SetPosition(0, endPos);
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        jitterTimer += Time.deltaTime;

        if (lr.material != null)
        {
            Vector2 offset = lr.material.mainTextureOffset;
            offset.x += textureScrollSpeed * Time.deltaTime;
            lr.material.mainTextureOffset = offset;
        }

        float refreshInterval = jitterRefreshRate <= 0f ? 999f : 1f / jitterRefreshRate;
        if (jitterTimer >= refreshInterval)
        {   
            jitterTimer = 0f;
            RebuildBeam();
        }

        if (lifeTimer >= duration)
        {   
            Destroy(gameObject);
        }
    }

    void RebuildBeam()
    {
        int count = Mathf.Max(2, segmentCount);
        lr.positionCount = count;

        Vector3 dir = (endPos - startPos).normalized;
        Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            Vector3 p = Vector3.Lerp(startPos, endPos, t);
            if (i != 0 && i != count - 1)
            {
                float edgeFade = Mathf.Sin(t * Mathf.PI);
                float offset = Random.Range(-jitterAmount, jitterAmount) * edgeFade;
                p += perp * offset;
            }
            lr.SetPosition(i, p);
        }
    }
}
