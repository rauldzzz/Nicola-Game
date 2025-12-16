using UnityEngine;
using UnityEngine.SceneManagement;

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

        SceneManager.LoadScene(sceneToLoad);
    }
}
