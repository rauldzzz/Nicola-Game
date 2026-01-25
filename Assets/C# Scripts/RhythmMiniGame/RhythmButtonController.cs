using UnityEngine;

public class RhythmButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;

    public Sprite defaultImage;
    public Sprite pressedImage;

    public KeyCode[] keysToPress;

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
        theSR.sprite = defaultImage;
    }

    void Update()
    {
        bool anyKeyDown = false;
        bool anyKeyHeld = false;

        foreach (KeyCode key in keysToPress)
        {
            if (Input.GetKeyDown(key))
                anyKeyDown = true;

            if (Input.GetKey(key))
                anyKeyHeld = true;
        }

        // If any assigned key was just pressed
        if (anyKeyDown)
        {
            theSR.sprite = pressedImage;
        }

        // If none of the assigned keys are held anymore
        if (!anyKeyHeld)
        {
            theSR.sprite = defaultImage;
        }
    }
}

