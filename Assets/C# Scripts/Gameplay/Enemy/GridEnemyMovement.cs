using UnityEngine;
using System.Collections;

/*
 * GridEnemyMovement
 * -----------------
 * A grid-based enemy that moves randomly along valid directions.
 * - Moves in discrete steps based on gridSize
 * - Avoids obstacles defined in obstacleLayer
 * - Can be stomped (killed) by the player, triggering a death animation with scaling & fading
 */
public class GridEnemyMovement : MonoBehaviour, IStompable
{
    [Header("Movement Settings")]
    public float gridSize = 1f;                 // Size of each grid cell
    public float moveSpeed = 3f;                // Speed of movement
    public LayerMask obstacleLayer;             // Layer mask for walls/obstacles

    [Header("Death Animation")]
    public float deathDuration = 2f;            // Time for the stomped death animation
    public float maxScaleIncrease = 1.5f;       // How big the enemy grows before fading out
    public SpriteRenderer spriteRenderer;       // Reference to sprite to fade out

    private Vector3 targetPosition;             // The position enemy is moving towards
    private bool isMoving = false;              // Flag for current movement
    private bool isDying = false;               // Flag if enemy is being destroyed
    private Collider2D col;                     // Collider reference

    void Start()
    {
        targetPosition = transform.position;    // Start at current position
        col = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDying) return; // Stop moving if dying

        if (!isMoving)
        {
            // Get all directions that are not blocked
            Vector3[] validDirs = GetValidDirections();

            if (validDirs.Length > 0)
            {
                // Pick a random valid direction
                Vector3 nextDir = validDirs[Random.Range(0, validDirs.Length)];
                Vector3 nextPos = targetPosition + nextDir * gridSize;

                // Start moving towards the target grid cell
                StartCoroutine(MoveToPosition(nextPos));
            }
        }
    }

    /// <summary>
    /// Returns array of directions that are free to move
    /// </summary>
    private Vector3[] GetValidDirections()
    {
        Vector3[] dirs = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        System.Collections.Generic.List<Vector3> valid = new System.Collections.Generic.List<Vector3>();

        foreach (var dir in dirs)
        {
            Vector3 nextPos = targetPosition + dir * gridSize;
            if (!IsBlocked(nextPos))
                valid.Add(dir);
        }

        return valid.ToArray();
    }

    /// <summary>
    /// Checks if the target position is blocked using the enemy collider and obstacleLayer
    /// </summary>
    private bool IsBlocked(Vector3 targetPos)
    {
        if (col == null) return false;

        // Use the enemy collider size with a slight margin
        Vector2 boxSize = col.bounds.size * 0.9f;
        Collider2D hit = Physics2D.OverlapBox(targetPos, boxSize, 0f, obstacleLayer);
        return hit != null;
    }

    /// <summary>
    /// Smoothly moves the enemy to a target grid position
    /// </summary>
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
    }

    // Called by player when stomped
    public void Stomped()
    {
        if (isDying) return;
        StartCoroutine(DeathRoutine());
    }

    /// <summary>
    /// Handles the stomp death animation: scales up and fades out
    /// </summary>
    private IEnumerator DeathRoutine()
    {
        isDying = true;

        if (col != null) col.enabled = false;

        float timer = 0f;
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer.color;

        while (timer < deathDuration)
        {
            float t = timer / deathDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * maxScaleIncrease, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Debug helper: visualize enemy collision box in editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size * 0.9f);
        }
    }
}