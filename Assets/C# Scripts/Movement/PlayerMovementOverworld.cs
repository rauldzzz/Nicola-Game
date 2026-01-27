using UnityEngine;
using System.Collections;

/*
 * GridMovementHold
 * ----------------
 * Handles smooth grid-based movement with held input.
 * - Moves the player tile by tile on a grid.
 * - Supports input buffering for responsive movement.
 * - Animates walking and idle states.
 * - Prevents movement into obstacles.
 * - Was used in an older version of the overworld scenes.
 * - Now its used in the FYR minigame.
 */

public class GridMovementHold : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;
    public float moveSpeed = 5f;
    public float inputBufferDistance = 0.5f; // How close to target we can accept new input

    private Vector3 targetPosition;
    public bool isMoving = false;
    private Vector2 inputDirection;
    private Vector2 lastMoveDirection = Vector2.down;

    [Header("Animation")]
    public Animator animator;

    public Vector2 InputDirection => inputDirection;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Snap player to nearest grid at start
        float snappedX = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(transform.position.y / gridSize) * gridSize;
        Vector3 snappedPosition = new Vector3(snappedX, snappedY, transform.position.z);

        transform.position = snappedPosition;
        targetPosition = snappedPosition;
    }

    void Update()
    {
        // Read directional input
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentInput = Vector2.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentInput = Vector2.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentInput = Vector2.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentInput = Vector2.right;

        if (currentInput != Vector2.zero)
        {
            // Allow new movement if player is stopped or close enough to target
            float distToTarget = Vector3.Distance(transform.position, targetPosition);
            bool nearTarget = distToTarget <= inputBufferDistance;

            if (!isMoving || nearTarget)
            {
                // Only try to move if input changed or not already moving
                if (currentInput != inputDirection || !isMoving)
                    TryMove(currentInput);
            }
        }

        UpdateAnimator(currentInput);
    }

    private void TryMove(Vector2 dir)
    {
        Vector3 nextPos = targetPosition + new Vector3(dir.x, dir.y, 0) * gridSize;

        // Don't move into obstacles
        Collider2D hit = Physics2D.OverlapCircle(nextPos, 0.2f, LayerMask.GetMask("Obstacles"));
        if (hit == null)
        {
            // Stop current movement so direction can change smoothly
            if (isMoving)
                StopAllCoroutines();

            inputDirection = dir;
            lastMoveDirection = dir;
            targetPosition = nextPos;

            StartCoroutine(MoveToPosition(nextPos));
        }
    }

    private IEnumerator MoveToPosition(Vector3 newPos)
    {
        isMoving = true;

        // Move smoothly to the target tile
        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap exactly to the grid
        transform.position = newPos;
        isMoving = false;

        // Check for held input to continue moving immediately
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
        animator.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            // Update direction while walking
            animator.SetFloat("InputX", inputDirection.x);
            animator.SetFloat("InputY", inputDirection.y);
        }
        else
        {
            // Idle animation uses last direction
            animator.SetFloat("LastInputX", lastMoveDirection.x);
            animator.SetFloat("LastInputY", lastMoveDirection.y);
        }
    }
}