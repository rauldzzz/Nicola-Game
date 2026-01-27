using UnityEngine;

/*
 * NoteObject
 * ----------
 * Handles player input for a single note.
 * - Detects key presses when the note is in the activator zone.
 * - Determines hit quality (Normal, Good, Perfect) based on distance to hit line.
 * - Plays corresponding visual effects.
 * - Calls RhythmGameManager to update score or register misses.
 */

public class NoteObject : MonoBehaviour
{
    [Header("Input")]
    public KeyCode[] keysToPress;

    [Header("Timing thresholds")]
    [Tooltip("World-space Y distance to hitline")]
    public float normalThreshold = 0.25f;
    public float goodThreshold = 0.05f;

    [Header("Effects")]
    public GameObject hitEffect, goodEffect, perfectEffect;

    [HideInInspector]
    public bool canBePressed = false;

    void Update()
    {
        if (!canBePressed)
            return;

        bool pressed = false;

        // Check if any assigned key is pressed
        foreach (KeyCode key in keysToPress)
        {
            if (Input.GetKeyDown(key))
            {
                pressed = true;
                break;
            }
        }

        if (!pressed)
            return;

        // Hide note once pressed
        gameObject.SetActive(false);

        // Calculate distance to hit line for judging accuracy
        float distance;
        GameObject go = GameObject.FindWithTag("Activator");

        if (go != null)
            distance = Mathf.Abs(transform.position.y - go.transform.position.y);
        else
            distance = Mathf.Abs(transform.position.y); // fallback

        // Score and spawn visual effects based on timing
        if (distance > normalThreshold)
        {
            RhythmGameManager.instance.NormalHit();
            if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        else if (distance > goodThreshold)
        {
            RhythmGameManager.instance.GoodHit();
            if (goodEffect) Instantiate(goodEffect, transform.position, Quaternion.identity);
        }
        else
        {
            RhythmGameManager.instance.PerfectHit();
            if (perfectEffect) Instantiate(perfectEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
            canBePressed = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            if (canBePressed) // count as missed only if note was active
                RhythmGameManager.instance.NoteMissed();
            canBePressed = false;
        }
    }
}