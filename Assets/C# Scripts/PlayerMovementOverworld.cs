using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridMovementHold_Commented : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;          // Size of one tile/grid cell
    public float moveSpeed = 5f;         // Speed at which the player moves to the next tile
    public float moveDelay = 0.15f;      // Small delay between moves when holding a key

    [Header("Tilemaps")]
    public Tilemap obstacleTilemap;      // Reference to the tilemap that contains obstacles

    private Vector3 targetPosition;      // The next grid position the player will move to
    private bool isMoving = false;       // Is the player currently moving? Prevents overlapping moves

    void Start()
    {
        // Initialize the target position as the starting position
        targetPosition = transform.position;
    }

    void Update()
    {
        // Only check for input if the player is not currently moving
        if (!isMoving)
        {
            Vector3 inputDirection = Vector3.zero; // Direction we want to move in this frame

            // Check which key is being held down
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                inputDirection = Vector3.up;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                inputDirection = Vector3.down;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                inputDirection = Vector3.left;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                inputDirection = Vector3.right;

            if (inputDirection != Vector3.zero)
            {
                // Convert the next position into a cell coordinate on the obstacle tilemap
                Vector3Int nextCell = obstacleTilemap.WorldToCell(targetPosition + inputDirection * gridSize);

                // Only move if there is NO obstacle tile in the target cell
                if (!obstacleTilemap.HasTile(nextCell))
                {
                    Vector3 newPos = targetPosition + inputDirection * gridSize;
                    
                    newPos = RoundToHalf(newPos);

                    // Start the coroutine to move the player smoothly to the next tile
                    StartCoroutine(MoveToPosition(newPos));
                }
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 newPos)
    {
        // Set the moving flag so no new input is processed until movement finishes
        isMoving = true;

        // Move the player smoothly to the target position over time
        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.deltaTime);
            yield return null; // Wait until next frame
        }

        // Snap exactly to the target position
        transform.position = newPos;

        // Update targetPosition so the next movement starts from here
        targetPosition = newPos;

        // Reset moving flag to allow next input
        isMoving = false;

        // Small delay before allowing the next move to make holding keys feel natural
        yield return new WaitForSeconds(moveDelay);
    }

    // Helper function: rounds each component to the nearest 0.5
    private Vector3 RoundToHalf(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x * 2f) / 2f;
        pos.y = Mathf.Round(pos.y * 2f) / 2f;
        pos.z = Mathf.Round(pos.z * 2f) / 2f;
        return pos;
    }
}
