using UnityEngine;

/*
 * NoteSpawner
 * -----------
 * Spawns notes based on a beatmap (array of NoteData).
 * - Each note spawns ahead of the hit line based on `noteTravelTime`.
 * - Waits for the rhythm game to start before spawning.
 */

public class NoteSpawner : MonoBehaviour
{
    public AudioSource music;
    public GameObject[] notePrefabs;   // 0=Left, 1=Down, 2=Up, 3=Right
    public Transform[] spawnPoints;    // Corresponding spawn points for each lane
    public NoteData[] notes;           // Beatmap data

    public float noteTravelTime = 1.5f; // How far ahead notes spawn

    private int nextIndex = 0;

    void Update()
    {
        if (!RhythmGameManager.instance.startPlaying)
            return; // Do nothing until gameplay starts

        float songTime = music.time;

        // Spawn notes that are due, accounting for travel time
        while (nextIndex < notes.Length &&
               songTime >= notes[nextIndex].time - noteTravelTime)
        {
            SpawnNote(notes[nextIndex]);
            nextIndex++;
        }
    }

    void SpawnNote(NoteData data)
    {
        // Spawn the note prefab at the correct lane
        Instantiate(
            notePrefabs[data.lane],
            spawnPoints[data.lane].position,
            Quaternion.identity
        );
    }
}