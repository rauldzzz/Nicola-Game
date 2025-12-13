using UnityEngine;
using System.Collections;

public class PlayerAttack_GridAnimated : MonoBehaviour
{
    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.Space;
    public float attackDuration = 0.2f;        // How long the attack hitbox lasts
    public float attackRange = 0.5f;           // Distance of attack hitbox

    [Header("Attack Visual Offset")]
    public float attackVisualOffset = 0.2f; // Distance the sprite is offset from the hitbox

    [Header("Attack Feedback")]
    public GameObject attackObject;            // Child object with SpriteRenderer + Animator

    [Header("Movement Reference")]
    public GridMovementHold movementScript;

    [Header("Hitboxsize")]
    public float width = 0.8f;
    public float height = 0.8f;

    private Animator attackAnimator;
    private SpriteRenderer attackSprite;
    private bool isAttacking = false;
    private Vector2 lastMoveDirection = Vector2.down;

    void Start()
    {
        if (attackObject != null)
        {
            attackAnimator = attackObject.GetComponent<Animator>();
            attackSprite = attackObject.GetComponent<SpriteRenderer>();
            if (attackSprite != null) attackSprite.enabled = false;
        }
    }

    void Update()
    {
        // Track last movement direction
        Vector2 inputDir = movementScript.InputDirection; // neue InputDirection aus MovementScript
        if (inputDir != Vector2.zero)
            lastMoveDirection = inputDir;

        if (Input.GetKeyDown(attackKey) && !isAttacking && attackObject != null)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Calculate attack position
        Vector3 attackPos = transform.position + new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0).normalized * attackRange;

        // Move and rotate the attackObject to match direction
        Vector3 visualPos = attackPos + new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0).normalized * attackVisualOffset;
        attackObject.transform.position = visualPos;

        if (lastMoveDirection == Vector2.right) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (lastMoveDirection == Vector2.left) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
        else if (lastMoveDirection == Vector2.up) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        else if (lastMoveDirection == Vector2.down) attackObject.transform.localRotation = Quaternion.Euler(0, 0, -90);

        attackSprite.enabled = true;
        attackAnimator.Play("AttackAnimation", -1, 0f);

        Vector2 hitboxSize = new Vector2(width, height);
        float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPos, hitboxSize, angle);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                IStompable stompable = hit.GetComponent<IStompable>();
                if (stompable != null)
                    stompable.Stomped();
                else
                    Destroy(hit.gameObject);
            }
        }

        yield return new WaitForSeconds(attackDuration);

        attackSprite.enabled = false;
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Vector3 attackPos = transform.position + new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0).normalized * attackRange;
        Gizmos.DrawWireSphere(attackPos, 0.2f);
    }
}
