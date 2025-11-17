using UnityEngine;


public class BeatScroller : MonoBehaviour
{
    [Tooltip("Song BPM")]
    public float BPM = 98f;
    public float travelDistance = 1.5f;

    public bool startWithSong = true;

    private float speed; // units per second
    private bool hasStarted = false;

    void Start()
    {
       
        float secondsPerBeat = 60f / Mathf.Max(0.0001f, BPM);
        speed = travelDistance / secondsPerBeat;
    }

    void Update()
    {
        if (startWithSong && !hasStarted && RhythmGameManager.instance != null)
            hasStarted = RhythmGameManager.instance.startPlaying;

        if (hasStarted)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    public void Begin()
    {
        hasStarted = true;
    }
}