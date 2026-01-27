using UnityEngine;
using UnityEngine.Events;

/*
 * ActionInteractable
 * ------------------
 * Handles objects the player can interact with by triggering UnityEvents.
 * - Can be set to allow interaction only once.
 * - Invokes assigned actions when interacted with.
 */

public class ActionInteractable : MonoBehaviour, IInteractable
{
    [Header("Interactions")]
    [Tooltip("If true, the player can interact with this only once.")]
    public bool interactOnlyOnce = false;

    [Header("The Action List")]
    public UnityEvent OnInteract;

    private bool _hasInteracted = false;

    public bool CanInteract()
    {
        if (interactOnlyOnce && _hasInteracted) return false;
        return true;
    }

    public void Interact()
    {
        OnInteract?.Invoke();
        _hasInteracted = true;
    }
}