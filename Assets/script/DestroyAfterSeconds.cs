using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float lifeTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
