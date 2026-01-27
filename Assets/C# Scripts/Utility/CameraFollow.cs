using UnityEngine;

/*
 * CameraFollow
 * ------------
 * Smoothly follows a target (usually the player) with an optional offset.
 * - Uses linear interpolation for smooth movement.
 * - Adjustable follow speed and offset.
 */

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // The object the camera should follow
    public float smoothSpeed = 5f; // How quickly the camera catches up
    public Vector3 offset;         // Offset from the target's position

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the target position with offset
            Vector3 desiredPosition = target.position + offset;

            // Smoothly interpolate from current to target position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Apply the smoothed position
            transform.position = smoothedPosition;
        }
    }
}