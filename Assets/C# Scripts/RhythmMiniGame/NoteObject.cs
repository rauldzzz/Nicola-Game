using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [Header("Input")]
    public KeyCode[] keysToPress;

    [Header("Timing thresholds (units: world-space Y distance to hitline)")]
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

        gameObject.SetActive(false);

        // Find the activator once per hit
        float distance;
        GameObject go = GameObject.FindWithTag("Activator");

        if (go != null)
            distance = Mathf.Abs(transform.position.y - go.transform.position.y);
        else
            distance = Mathf.Abs(transform.position.y); // fallback

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
            if (canBePressed) // only count as missed if it was activatable
                RhythmGameManager.instance.NoteMissed();
            canBePressed = false;
        }
    }
}