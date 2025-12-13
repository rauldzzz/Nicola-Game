using UnityEngine;
using System.Collections;

public class GridMovementHold : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;       // Size of one tile/grid cell
    public float moveSpeed = 5f;      // Speed at which the player moves to the next tile
    public float moveDelay = 0.15f;   // Small delay between moves when holding a key

    private Vector3 targetPosition;   // The next grid position the player will move to
    private bool isMoving = false;    // Is the player currently moving? Prevents overlapping moves
    private Vector3 lastPosition;     // Used to detect if the player was teleported

    public bool IsMoving => isMoving;

    void Start()
    {
        // Initialize the target position as the starting position
        targetPosition = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Detect if the player has been teleported (position changed externally)
        if (Vector3.Distance(transform.position, lastPosition) > gridSize * 0.1f && !isMoving)
        {
            targetPosition = transform.position;
        }

        if (!isMoving)
        {
            Vector3 inputDirection = GetInputDirection();

            if (inputDirection != Vector3.zero)
            {
                Vector3 nextPos = targetPosition + inputDirection * gridSize;

                // Check for obstacles using Physics2D
                Collider2D hit = Physics2D.OverlapCircle(nextPos, 0.2f, LayerMask.GetMask("Obstacles"));
                if (hit == null)
                {
                    StartCoroutine(MoveToPosition(nextPos));
                }
            }
        }

        lastPosition = transform.position;
    }

    // Helper method to get input direction
    private Vector3 GetInputDirection()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return Vector3.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) return Vector3.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) return Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) return Vector3.right;

        return Vector3.zero;
    }

    private IEnumerator MoveToPosition(Vector3 newPos)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = newPos;
        targetPosition = newPos;
        isMoving = false;

        yield return new WaitForSeconds(moveDelay);
    }
}