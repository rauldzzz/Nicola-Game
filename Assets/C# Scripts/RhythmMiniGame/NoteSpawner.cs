using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public AudioSource music;
    public GameObject[] notePrefabs;   // 0=Left, 1=Down, 2=Up, 3=Right
    public Transform[] spawnPoints;    // Transforms for each lane
    public NoteData[] notes;           // Your beatmap

    public float noteTravelTime = 1.5f;

    private int nextIndex = 0;

    void Update()
    {
        if (!RhythmGameManager.instance.startPlaying)
            return; // Do NOTHING until the game starts

        float songTime = music.time;

        // Spawn notes when it's time
        while (nextIndex < notes.Length &&
               songTime >= notes[nextIndex].time - noteTravelTime)
        {
            SpawnNote(notes[nextIndex]);
            nextIndex++;
        }
    }

    void SpawnNote(NoteData data)
    {
        Instantiate(
            notePrefabs[data.lane],
            spawnPoints[data.lane].position,
            Quaternion.identity
        );
    }
}