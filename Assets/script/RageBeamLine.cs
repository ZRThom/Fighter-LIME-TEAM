using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RageBeamLine : MonoBehaviour
{
    public float duration = 0.5f;
    public float extraOffScreen = 2f;

    private LineRenderer lr;
    private float timer;
    private Vector3 startPos;
    private Vector3 endPos;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
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

        startPos = goRight ? new Vector3(left.x - extraOffScreen, yWorld, transform.position.z) : new Vector3(right.x + extraOffScreen, yWorld, transform.position.z);
        endPos = goRight ? new Vector3(right.x + extraOffScreen, yWorld, transform.position.z) : new Vector3(left.x - extraOffScreen, yWorld, transform.position.z);
    
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {   
            Destroy(gameObject);
        }
    }
}
