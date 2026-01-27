using UnityEngine;

/*
 * MusicManager
 * ------------
 * Keeps a single instance of background music playing across scenes.
 * - Uses a static singleton reference to prevent duplicates.
 * - Music persists between scene loads.
 */

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Keep only one instance of the music manager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep music playing between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if another instance exists
        }
    }
}