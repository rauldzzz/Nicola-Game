using UnityEngine;

/*
 * BeatScroller
 * ------------
 * Moves notes downward at a consistent speed to match the song's BPM.
 * - Calculates speed based on travel distance and beats per minute.
 * - Can start automatically with the song or be triggered manually.
 */

public class BeatScroller : MonoBehaviour
{
    [Tooltip("Song BPM")]
    public float BPM = 98f;
    public float travelDistance = 1.5f; // Distance notes travel before hitting the target

    public bool startWithSong = true;

    private float speed;      // Units per second
    private bool hasStarted = false;

    void Start()
    {
        // Calculate speed so notes reach the hit line in sync with the beat
        float secondsPerBeat = 60f / Mathf.Max(0.0001f, BPM);
        speed = travelDistance / secondsPerBeat;
    }

    void Update()
    {
        // Automatically start scrolling when the rhythm game begins
        if (startWithSong && !hasStarted && RhythmGameManager.instance != null)
            hasStarted = RhythmGameManager.instance.startPlaying;

        if (hasStarted)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    // Can be called externally to start the scrolling manually
    public void Begin()
    {
        hasStarted = true;
    }
}