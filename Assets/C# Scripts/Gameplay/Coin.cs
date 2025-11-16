using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1; // How much this coin adds to the counter
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Increase coin count
            CoinManager.Instance.AddCoins(coinValue);

            // Play sound
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}
