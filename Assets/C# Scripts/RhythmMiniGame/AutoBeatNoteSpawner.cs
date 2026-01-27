using UnityEngine;

/*
 * AutoBeatNoteSpawner
 * ------------------
 * Automatically spawns notes in a rhythm game based on the music's BPM.
 * - Supports optional half- and quarter-beats.
 * - Each beat can have a chance to spawn a note.
 * - Notes move toward a hit line over a set travel time.
 */

public class AutoBeatNoteSpawner : MonoBehaviour
{
    public AudioSource music;
    public float BPM = 98f;
    public float noteTravelTime = 1.5f;
    public float firstBeatTime = 0f;

    public GameObject[] notePrefabs;    // Note prefabs for each lane
    public Transform[] spawnPoints;     // Spawn points matching the note prefabs
    public Transform hitLine;           // Target line for the notes

    public bool useHalfBeats = false;
    public bool useQuarterBeats = false;

    public float chancePerBeat = 1f;   // 1 = always spawn on a beat

    private float beatInterval;         // Seconds per beat
    private float nextSpawnTime;        // Song time when next note should spawn

    void Start()
    {
        // Ensure arrays are set up correctly
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

        // Spawn notes when it's time (considering travel time)
        while (songTime >= nextSpawnTime - noteTravelTime)
        {
            if (Random.value <= chancePerBeat)
                SpawnRandomLane();

            // Adjust interval for half/quarter beats if needed
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
        note.transform.SetParent(null); // Ensure note isn't parented to the spawner

        // Initialize note movement toward hit line
        NoteMovement nm = note.GetComponent<NoteMovement>();
        if (nm != null)
            nm.Initialize(spawnPos, hitLine != null ? hitLine.position : Vector3.zero, noteTravelTime);
        else
            Debug.LogWarning("Spawned note prefab does not contain NoteMovement component.");
    }
}