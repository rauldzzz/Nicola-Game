using UnityEngine;

/*
 * TextureScroller
 * ----------------
 * Scrolls the texture of a material vertically to create the illusion
 * of movement based on the current game speed.
 */
public class TextureScroller : MonoBehaviour
{
    private Renderer rend;
    
    // Tracks the cumulative offset of the texture to apply smooth scrolling
    private float currentOffset = 0f;

    void Start()
    {
        // Cache the Renderer component for efficiency
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Get the current game speed from the BusGameManager
        float currentSpeed = BusGameManager.Instance.gameSpeed;

        // Adjust scrolling speed relative to the object's scale
        float textureSpeed = currentSpeed / transform.localScale.y;

        // Increment the offset based on this frame's movement
        currentOffset += textureSpeed * Time.deltaTime;

        // Wrap offset to stay between 0 and 1 to avoid floating-point overflow
        currentOffset = currentOffset % 1;

        // Apply the offset to the material's main texture
        rend.material.mainTextureOffset = new Vector2(0, currentOffset);
    }
}
