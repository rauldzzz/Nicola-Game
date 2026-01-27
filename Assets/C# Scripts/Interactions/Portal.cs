using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Portal
 * ------
 * Implements IInteractable to allow the player to transition between scenes.
 * - Loads a specified scene when interacted with.
 * - Optionally saves the player's overworld position if this is a level entrance.
 * - Updates the last visited level in the SaveManager.
 * - Logs warnings if required references are missing.
 */

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Scene Settings")]
    public string sceneToLoad;
    public bool isLevelEntrance = true;

    public bool CanInteract() => true;

    public void Interact()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Portal: sceneToLoad is not set!");
            return;
        }

        // Save player position before loading a new level if this portal is an entrance
        if (isLevelEntrance)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveOverworldPosition(player.transform.position);
                SaveManager.Instance.lastVisitedLevel = sceneToLoad;
            }
            else
            {
                Debug.LogWarning("Portal: Player or SaveManager not found!");
            }
        }

        // Load the specified scene
        SceneManager.LoadScene(sceneToLoad);
    }
}