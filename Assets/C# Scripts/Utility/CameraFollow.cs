using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // Player-Transform
    public float smoothSpeed = 5f;   // Follow Speed
    public Vector3 offset;           // Distance to Player

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
