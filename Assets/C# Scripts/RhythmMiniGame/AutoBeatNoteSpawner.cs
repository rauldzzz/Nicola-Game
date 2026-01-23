using UnityEngine;

public class AutoBeatNoteSpawner : MonoBehaviour
{
    public AudioSource music;
    public float BPM = 98f;
    public float noteTravelTime = 1.5f;
    public float firstBeatTime = 0f;

    public GameObject[] notePrefabs;    
    public Transform[] spawnPoints;     
    public Transform hitLine;           

    public bool useHalfBeats = false;
    public bool useQuarterBeats = false;

    public float chancePerBeat = 1f; // chance to spawn a note on a beat (1 = every beat)

    private float beatInterval;   // seconds per beat
    private float nextSpawnTime;  // absolute song time (seconds) when next note should be spawned

    void Start()
    {
        if (notePrefabs == null || notePrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length != notePrefabs.Length)
        {
            Debug.LogError("AutoBeatNoteSpawner: Please assign notePrefabs and spawnPoints of equal length.");
            enabled = false;
            return;
        }

        beatInterval = 60f / Mathf.Max(0.0001f, BPM);
        nextSpawnTime = firstBeatTime;
    }

    void Update()
    {
        if (RhythmGameManager.instance == null || !RhythmGameManager.instance.startPlaying) return;
        if (music == null) return;

        float songTime = music.time;

        while (songTime >= nextSpawnTime - noteTravelTime)
        {
            if (Random.value <= chancePerBeat)
                SpawnRandomLane();

            float increment = beatInterval;
            if (useQuarterBeats) increment = beatInterval / 4f;
            else if (useHalfBeats) increment = beatInterval / 2f;

            nextSpawnTime += increment;
        }
    }

    private void SpawnRandomLane()
    {
        int lane = Random.Range(0, notePrefabs.Length);
        SpawnNoteInLane(lane);
    }

    private void SpawnNoteInLane(int lane)
    {
        if (lane < 0 || lane >= spawnPoints.Length) return;

        Vector3 spawnPos = spawnPoints[lane].position;
        GameObject note = Instantiate(notePrefabs[lane], spawnPos, Quaternion.identity);
        note.transform.SetParent(null); 

        NoteMovement nm = note.GetComponent<NoteMovement>();
        if (nm != null)
        {
            nm.Initialize(spawnPos, hitLine != null ? hitLine.position : Vector3.zero, noteTravelTime);
        }
        else
        {
            Debug.LogWarning("Spawned note prefab does not contain NoteMovement component.");
        }
    }
}