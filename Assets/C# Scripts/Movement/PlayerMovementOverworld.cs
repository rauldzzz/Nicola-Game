using UnityEngine;
using System.Collections;

public class GridMovementHold : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;              // Size of one grid tile
    public float moveSpeed = 5f;             // Movement speed between tiles

    // Distance before reaching the target where new input is accepted
    public float inputBufferDistance = 0.5f;

    private Vector3 targetPosition;          // Current target grid position
    public bool isMoving = false;             // Is the player currently moving
    private Vector2 inputDirection;           // Current movement direction
    private Vector2 lastMoveDirection = Vector2.down; // Last direction used (for idle)

    [Header("Animation")]
    public Animator animator;

    // Expose movement direction for other scripts (attack, interaction, etc.)
    public Vector2 InputDirection => inputDirection;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Snap the player to the nearest grid position on start
        float snappedX = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(transform.position.y / gridSize) * gridSize;

        Vector3 snappedPosition = new Vector3(snappedX, snappedY, transform.position.z);

        transform.position = snappedPosition;
        targetPosition = snappedPosition;
    }

    void Update()
    {
        // Read continuous movement input
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentInput = Vector2.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentInput = Vector2.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentInput = Vector2.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentInput = Vector2.right;

        if (currentInput != Vector2.zero)
        {
            // Check how close we are to the current target tile
            float distToTarget = Vector3.Distance(transform.position, targetPosition);
            bool nearTarget = distToTarget <= inputBufferDistance;

            // Allow new movement if stopped or almost at the target
            if (!isMoving || nearTarget)
            {
                // Prevent repeating the same move unnecessarily
                if (currentInput != inputDirection || !isMoving)
                {
                    TryMove(currentInput);
                }
            }
        }

        UpdateAnimator(currentInput);
    }

    private void TryMove(Vector2 dir)
    {
        // Calculate the next grid position
        Vector3 nextPos = targetPosition + new Vector3(dir.x, dir.y, 0) * gridSize;

        // Check if the target tile is blocked
        Collider2D hit = Physics2D.OverlapCircle(nextPos, 0.2f, LayerMask.GetMask("Obstacles"));

        if (hit == null)
        {
            // Stop current movement to allow smooth direction changes
            if (isMoving)
                StopAllCoroutines();

            // Store movement direction
            inputDirection = dir;
            lastMoveDirection = dir;
            targetPosition = nextPos;

            // Start moving towards the new tile
            StartCoroutine(MoveToPosition(nextPos));
        }
    }

    private IEnumerator MoveToPosition(Vector3 newPos)
    {
        isMoving = true;

        // Smoothly move toward the target tile
        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                newPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Snap exactly to grid
        transform.position = newPos;
        isMoving = false;

        // Immediately check for held input to continue moving
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentInput = Vector2.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentInput = Vector2.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentInput = Vector2.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentInput = Vector2.right;

        if (currentInput != Vector2.zero)
            TryMove(currentInput);
    }

    private void UpdateAnimator(Vector2 currentInput)
    {
        // Set walking state
        animator.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            // Update movement direction while walking
            animator.SetFloat("InputX", inputDirection.x);
            animator.SetFloat("InputY", inputDirection.y);
        }
        else
        {
            // Use last movement direction for idle animations
            animator.SetFloat("LastInputX", lastMoveDirection.x);
            animator.SetFloat("LastInputY", lastMoveDirection.y);
        }
    }
}
