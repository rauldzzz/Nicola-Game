using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridEnemyMovement : MonoBehaviour
{
    public float gridSize = 1f;
    public float moveSpeed = 3f;
    public Tilemap obstacleTilemap;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
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
}
