using UnityEngine;

public class TriggeredInfoPanel : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerScript;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        if (playerScript != null)
        {
            playerRb = playerScript.GetComponent<Rigidbody2D>();
        }

        // Make sure the panel starts hidden
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        // Close the panel when E is pressed
        if (this.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ClosePanel();
        }
    }

    public void OpenPanel()
    {
        this.gameObject.SetActive(true);

        if (playerScript != null)
        {
            // Stop movement immediately
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }

            // Disable player input/movement
            playerScript.enabled = false;
        }
    }

    public void ClosePanel()
    {
        if (playerScript != null)
        {
            // Re-enable player input/movement
            playerScript.enabled = true;
        }

        this.gameObject.SetActive(false);
    }
}
