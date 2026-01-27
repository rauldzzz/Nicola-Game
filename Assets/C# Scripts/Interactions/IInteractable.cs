/*
 * IInteractable
 * -------------
 * Interface for objects that the player can interact with.
 * - `Interact()` is called when the player uses the object.
 * - `CanInteract()` returns whether the object is currently usable.
 */

public interface IInteractable
{
    void Interact();
    bool CanInteract();
}