using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // 1. Get the global speed
        float currentSpeed = BusGameManager.Instance.gameSpeed;

        // 2. Calculate how fast the texture should slide relative to the object's size
        // If the object is tall (Scale Y is high), the texture needs to scroll slower to cover the same distance.
        float textureSpeed = currentSpeed / transform.localScale.y;

        // 3. Apply the offset
        // We use repeat (modulo 1) to keep the numbers small, though usually not strictly necessary
        float offset = (Time.time * textureSpeed) % 1;

        rend.material.mainTextureOffset = new Vector2(0, offset); // Negative to move down
    }
}