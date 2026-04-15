using UnityEngine;
using System.Collections;

public class CameraShake2d : MonoBehaviour
{
    public static CameraShake2d Instance;
    private Coroutine shakeRoutine;
    private Vector3 shakeOffset = Vector3.zero;

    void Awake()
    {
        Instance = this;
    }

    void LateUpdate()
    {
        transform.position += shakeOffset;
    }

    public void Shake(float intensity, float duration)
    {
        
    }
}
