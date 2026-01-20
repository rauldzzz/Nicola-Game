using UnityEngine;
using UnityEngine.Events;

public class ActionInteractable : MonoBehaviour, IInteractable
{
    [Header("Interactions")]
    [Tooltip("Check this if the player should only be able to interact once.")]
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