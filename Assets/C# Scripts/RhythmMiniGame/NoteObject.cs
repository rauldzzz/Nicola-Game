using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [Header("Input")]
    public KeyCode keyToPress;

    [Header("Timing thresholds (units: world-space Y distance to hitline)")]
    public float normalThreshold = 0.25f;
    public float goodThreshold = 0.05f;

    [Header("Effects")]
    public GameObject hitEffect, goodEffect, perfectEffect;

    [HideInInspector]
    public bool canBePressed = false;

    void Update()
    {
        if (Input.GetKeyDown(keyToPress) && canBePressed)
        {
            // hide or destroy note
            gameObject.SetActive(false);

            // judging by vertical distance (assuming hitLine y == 0 world-space or use reference)
            float distance = 0f;
            // If the hitline has a known Y, better to compute: distance = Mathf.Abs(transform.position.y - hitLineY)
            // We'll assume the Activator's Y is zero; if not, adjust this accordingly.
            // For flexibility, we can try find the activator in the scene:
            Transform activator = null;
            var go = GameObject.FindWithTag("Activator");
            if (go != null) activator = go.transform;

            if (activator != null)
                distance = Mathf.Abs(transform.position.y - activator.position.y);
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