using UnityEngine;
using System.Collections;

public class GridMovementHold : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;          // Size of one tile/grid cell
    public float moveSpeed = 5f;         // Speed at which the player moves to the next tile
    public float moveDelay = 0.15f;      // Small delay between moves when holding a key

    private Vector3 targetPosition;      // The next grid position the player will move to
    private bool isMoving = false;       // Is the player currently moving? Prevents overlapping moves
    private Vector3 lastPosition;        // Used to detect if the player was teleported
    private Vector2 inputDirection;      // Current input direction

    [Header("Animation")]
    public Animator animator;

    public bool IsMoving => isMoving;

    void Start()
    {
        targetPosition = transform.position;
        lastPosition = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();
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
            inputDirection = Vector2.zero;

            // Detect key input
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                inputDirection = Vector2.up;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                inputDirection = Vector2.down;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                inputDirection = Vector2.left;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                inputDirection = Vector2.right;

            if (inputDirection != Vector2.zero)
            {
                // Check for obstacles
                Vector3 nextPos = targetPosition + new Vector3(inputDirection.x, inputDirection.y, 0) * gridSize;
                Collider2D hit = Physics2D.OverlapCircle(nextPos, 0.2f, LayerMask.GetMask("Obstacles"));

                if (hit == null)
                {
                    StartCoroutine(MoveToPosition(nextPos));
                }
            }
        }

        UpdateAnimator();

        lastPosition = transform.position;
    }

    private void UpdateAnimator()
    {
        // Set walking state
        animator.SetBool("IsWalking", isMoving);

        // Set direction for animations when moving
        if (isMoving)
        {
            animator.SetFloat("InputX", inputDirection.x);
            animator.SetFloat("InputY", inputDirection.y);
        }
        else if (inputDirection != Vector2.zero)
        {
            // Remember last direction when stopping
            animator.SetFloat("LastInputX", inputDirection.x);
            animator.SetFloat("LastInputY", inputDirection.y);
        }
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