using UnityEngine;

/*
 * ForceTimeScale
 * --------------
 * Ensures that the game's timescale is reset to normal (1) when this object is initialized.
 * Useful for scenes where Time.timeScale might have been modified (e.g., after a pause or cutscene).
 */
public class ForceTimeScale : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 1f; // Reset time to normal speed
    }
}