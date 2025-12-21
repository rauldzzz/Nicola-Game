using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    public PopupManager popupManager;
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            popupManager.ShowPopup();
        }
    }
}
