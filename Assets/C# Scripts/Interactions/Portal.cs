using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Scene Settings")]
    public string sceneToLoad;       // Name of target scene
    public bool isLevelEntrance = true;  // True = going FROM overworld TO level

    // Always interactable
    public bool CanInteract()
    {
        return true;
    }

    // Trigger scene load
    public void Interact()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Portal: sceneToLoad is not set!");
            return;
        }

        // Going from overworld into a level
        if (isLevelEntrance)
        {
            SaveManager.Instance.overworldPlayerPosition = 
                OverworldPlayerReference().position;

            SaveManager.Instance.lastVisitedLevel = sceneToLoad;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private Transform OverworldPlayerReference()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player.transform;
    }
}
