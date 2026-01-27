using UnityEngine;

/*
 * RhythmButtonController
 * ----------------------
 * Handles the visual feedback for a single rhythm button.
 * - Changes the button sprite when keys are pressed or held.
 * - Supports multiple keys for the same button.
 */

public class RhythmButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;

    public Sprite defaultImage;
    public Sprite pressedImage;

    public KeyCode[] keysToPress;

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
        theSR.sprite = defaultImage; // Start with default button sprite
    }

    void Update()
    {
        bool anyKeyDown = false;
        bool anyKeyHeld = false;

        // Check all assigned keys
        foreach (KeyCode key in keysToPress)
        {
            if (Input.GetKeyDown(key))
                anyKeyDown = true;

            if (Input.GetKey(key))
                anyKeyHeld = true;
        }

        // Show pressed sprite if any key was just pressed
        if (anyKeyDown)
        {
            theSR.sprite = pressedImage;
        }

        // Revert to default sprite when no keys are held
        if (!anyKeyHeld)
        {
            theSR.sprite = defaultImage;
        }
    }
}