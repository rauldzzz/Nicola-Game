using UnityEngine;
using System.Collections;

public class GridMovementHold : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;
    public float moveSpeed = 5f;

    // A small distance buffer to allow input for the next move
    // We start looking for the next move when we are this close to the target.
    public float inputBufferDistance = 0.5f;

    private Vector3 targetPosition;
    public bool isMoving = false;
    private Vector2 inputDirection;
    private Vector2 lastMoveDirection = Vector2.down;

    [Header("Animation")]
    public Animator animator;

    // Expose input direction for other scripts
    public Vector2 InputDirection => inputDirection;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // 1. Calculate the snapped, grid-aligned position
        float snappedX = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(transform.position.y / gridSize) * gridSize;

        Vector3 snappedPosition = new Vector3(snappedX, snappedY, transform.position.z);

        // 2. Immediately set the character's position and the target position 
        // to this perfectly aligned point.
        transform.position = snappedPosition;
        targetPosition = snappedPosition;
    }

    void Update()
    {
        // 1. Read continuous input (which keys are down)
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentInput = Vector2.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentInput = Vector2.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentInput = Vector2.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentInput = Vector2.right;

        if (currentInput != Vector2.zero)
        {
            // 2. Check if we are ready to accept new input
            // We are ready if:
            // a) We are currently NOT moving (regular discrete move)
            // b) OR if we ARE moving, but we are very close to the current target position
            float distToTarget = Vector3.Distance(transform.position, targetPosition);
            bool nearTarget = distToTarget <= inputBufferDistance;

            if (!isMoving || nearTarget)
            {
                // Only try to move if the input is different OR if we just stopped moving
                // This prevents redundant checks if we are still far from the target
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
        Vector3 nextPos = targetPosition + new Vector3(dir.x, dir.y, 0) * gridSize;

        // Check for obstacles from the *new* target position
        Collider2D hit = Physics2D.OverlapCircle(nextPos, 0.2f, LayerMask.GetMask("Obstacles"));

        if (hit == null)
        {
            // If we are currently moving, stop the previous MoveToPosition Coroutine
            // This allows us to smoothly change direction mid-tile.
            if (isMoving)
            {
                StopAllCoroutines();
            }

            // Set the new target and direction
            inputDirection = dir;
            lastMoveDirection = dir;
            targetPosition = nextPos;

            // Start the movement coroutine to the new target
            StartCoroutine(MoveToPosition(nextPos));
        }
    }

    private IEnumerator MoveToPosition(Vector3 newPos)
    {
        isMoving = true;

        // Loop until the distance is negligible
        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to the final position to ensure grid alignment
        transform.position = newPos;

        isMoving = false;

        // IMPORTANT: Check for immediate input again right after stopping
        // If the player is still holding a key, start the next move immediately
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) currentInput = Vector2.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) currentInput = Vector2.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) currentInput = Vector2.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) currentInput = Vector2.right;

        if (currentInput != Vector2.zero)
        {
            TryMove(currentInput);
        }
    }

    private void UpdateAnimator(Vector2 currentInput)
    {
        animator.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            // Moving → update current direction
            animator.SetFloat("InputX", inputDirection.x);
            animator.SetFloat("InputY", inputDirection.y);
        }
        else
        {
            // Idle → use last movement direction
            animator.SetFloat("LastInputX", lastMoveDirection.x);
            animator.SetFloat("LastInputY", lastMoveDirection.y);
        }
    }
}