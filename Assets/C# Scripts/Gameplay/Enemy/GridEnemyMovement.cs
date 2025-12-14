using UnityEngine;
using System.Collections;

public class GridEnemyMovement : MonoBehaviour, IStompable
{
    [Header("Movement Settings")]
    public float gridSize = 1f;
    public float moveSpeed = 3f;
    public LayerMask obstacleLayer;

    [Header("Death Animation")]
    public float deathDuration = 2f;
    public float maxScaleIncrease = 1.5f;
    public SpriteRenderer spriteRenderer;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isDying = false;
    private Collider2D col;

    void Start()
    {
        targetPosition = transform.position;
        col = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDying) return;

        if (!isMoving)
        {
            // Get list of valid directions that are not blocked
            Vector3[] validDirs = GetValidDirections();

            if (validDirs.Length > 0)
            {
                // Pick one at random
                Vector3 nextDir = validDirs[Random.Range(0, validDirs.Length)];
                Vector3 nextPos = targetPosition + nextDir * gridSize;
                StartCoroutine(MoveToPosition(nextPos));
            }
        }
    }

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

    private bool IsBlocked(Vector3 targetPos)
    {
        if (col == null) return false;

        // Use the enemy collider's size with a slight margin
        Vector2 boxSize = col.bounds.size * 0.9f;
        Collider2D hit = Physics2D.OverlapBox(targetPos, boxSize, 0f, obstacleLayer);
        return hit != null;
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
    }

    public void Stomped()
    {
        if (isDying) return;
        StartCoroutine(DeathRoutine());
    }

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

    // Debug: visualize the collision box in the editor
    void OnDrawGizmosSelected()
    {
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size * 0.9f);
        }
    }
}