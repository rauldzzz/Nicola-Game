using UnityEngine;


public class NoteMovement : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float travelTime = 1.5f;
    private float elapsed = 0f;
    private float speed = 0f;
    private bool initialized = false;

    public void Initialize(Vector3 spawnPosition, Vector3 hitPosition, float noteTravelTime)
    {
        startPos = spawnPosition;
        targetPos = hitPosition;
        travelTime = Mathf.Max(0.0001f, noteTravelTime);
        float distance = Vector3.Distance(startPos, targetPos);
        speed = distance / travelTime;
        transform.position = startPos;
        elapsed = 0f;
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        // Move toward target using MoveTowards for stable, constant speed
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        elapsed += Time.deltaTime;
        // Optionally destroy when it reaches the target
        if (Vector3.Distance(transform.position, targetPos) <= 0.01f || elapsed >= travelTime + 0.5f)
        {
            // When hits the hitline zone without being pressed, the NoteObject's trigger should handle misses
            Destroy(gameObject, 0.1f);
        }
    }
}