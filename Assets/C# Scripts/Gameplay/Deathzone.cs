using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the player touches the death zone
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has died — respawning...");
            RespawnManager.Instance.RespawnPlayer(other.gameObject);
        }
    }
}
