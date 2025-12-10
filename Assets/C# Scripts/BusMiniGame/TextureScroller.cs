using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    private Renderer rend;
    private float currentOffset = 0f; // We track the offset ourselves now

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float currentSpeed = BusGameManager.Instance.gameSpeed;

        // Calculate how much to move THIS frame only
        float textureSpeed = currentSpeed / transform.localScale.y;

        // Add this tiny movement to our total offset
        currentOffset += textureSpeed * Time.deltaTime;

        // Keep the number between 0 and 1 so it doesn't get too big over time
        currentOffset = currentOffset % 1;

        // Apply it
        rend.material.mainTextureOffset = new Vector2(0, currentOffset);
    }
}