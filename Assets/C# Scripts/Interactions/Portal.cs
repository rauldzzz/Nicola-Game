using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Scene Settings")]
    public string sceneToLoad; // Set the target scene in the Inspector

    // Always interactable
    public bool CanInteract()
    {
        return true;
    }

    // Trigger scene load
    public void Interact()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Portal: sceneToLoad is not set!");
        }
    }
}
