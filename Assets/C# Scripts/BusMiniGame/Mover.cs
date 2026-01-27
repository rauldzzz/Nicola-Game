using UnityEngine;

/*
 * Mover
 * -----
 * Handles downward movement for obstacles in the bus minigame.
 * Movement speed is tied to the current game speed from BusGameManager.
 * Destroys the object once it goes off-screen.
 */
public class Mover : MonoBehaviour
{
    void Update()
    {
        // Get the current speed from the main game manager
        float currentSpeed = BusGameManager.Instance.gameSpeed;

        // Move the object downwards based on game speed
        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

        // Destroy the object when it goes below the visible screen
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
