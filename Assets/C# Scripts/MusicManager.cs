using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Keep only one instance of the music manager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps music playing between scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}
