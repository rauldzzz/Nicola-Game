using UnityEngine;

/*
 * NoteData
 * --------
 * Stores information for a single note in the rhythm game.
 * - `time`: when the note should appear (in seconds).
 * - `lane`: which lane the note belongs to.
 */

[System.Serializable]
public class NoteData
{
    public float time;
    public int lane;
}