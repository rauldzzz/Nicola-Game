using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridEnemyMovement : MonoBehaviour, IStompable
{
    public float gridSize = 1f;
    public float moveSpeed = 3f;
    public Tilemap obstacleTilemap;

    [Header("Death Animation")]
    public float deathDuration = 2f;            // how long the death animation lasts
    public float maxScaleIncrease = 1.5f;       // 1 = no growth, 1.5 = grow 50%
    public SpriteRenderer spriteRenderer;       // needed for fading out / scaling

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
        if (isDying) return;   // stop movement while dying

        if (!isMoving)
        {
            Vector3 nextDir = GetRandomDirection();
            Vector3 nextPos = targetPosition + nextDir * gridSize;

            Vector3Int cell = obstacleTilemap.WorldToCell(nextPos);
            if (!obstacleTilemap.HasTile(cell))
            {
                StartCoroutine(MoveToPosition(nextPos));
            }
        }
    }

    private Vector3 GetRandomDirection()
    {
        int r = Random.Range(0, 5);
        switch (r)
        {
            case 0: return Vector3.up;
            case 1: return Vector3.down;
            case 2: return Vector3.left;
            case 3: return Vector3.right;
            default: return Vector3.zero;
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
    }

    // -------- STOMP / DEATH LOGIC --------
    public void Stomped()
    {
        if (isDying) return;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;

        // disable collisions so he can't hurt the player
        if (col != null) col.enabled = false;

        float timer = 0f;
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer.color;

        // Animation: grow + fade
        while (timer < deathDuration)
        {
            float t = timer / deathDuration;

            // Scale up over time
            transform.localScale = Vector3.Lerp(originalScale, originalScale * maxScaleIncrease, t);

            // Fade out
            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                Mathf.Lerp(1f, 0f, t)
            );

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // enemy dies
    }
}
