using UnityEngine;
using System.Collections;

/*
 * PlayerAttack_GridAnimated
 * -------------------------
 * Handles player attacks in a grid-based system.
 * - Tracks last movement direction to orient attacks.
 * - Uses a child attackObject for visual feedback and animation.
 * - Activates a hitbox in front of the player for a short duration.
 * - Damages or "stomps" enemies within the hitbox.
 * - Prevents multiple attacks at once.
 */

public class PlayerAttack_GridAnimated : MonoBehaviour
{
    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.Space;       // Key to trigger attack
    public float attackDuration = 0.2f;            // Duration the attack hitbox is active
    public float attackRange = 0.5f;               // Distance from player center where hitbox appears

    [Header("Attack Visual Offset")]
    public float attackVisualOffset = 0.2f;        // How far the sprite is offset from the hitbox center

    [Header("Attack Feedback")]
    public GameObject attackObject;                // Child object with sprite + animator for attack animation

    [Header("Movement Reference")]
    public GridMovementHold movementScript;        // Reference to player movement script to get input direction

    [Header("Hitbox Size")]
    public float width = 0.8f;
    public float height = 0.8f;

    private Animator attackAnimator;
    private SpriteRenderer attackSprite;
    private bool isAttacking = false;             // Prevents multiple attacks at once
    private Vector2 lastMoveDirection = Vector2.down; // Defaults to facing down

    void Start()
    {
        if (attackObject != null)
        {
            attackAnimator = attackObject.GetComponent<Animator>();
            attackSprite = attackObject.GetComponent<SpriteRenderer>();
            if (attackSprite != null) attackSprite.enabled = false; // hide attack visual initially
        }
    }

    void Update()
    {
        // Track last movement direction for attack orientation
        Vector2 inputDir = movementScript.InputDirection;
        if (inputDir != Vector2.zero)
            lastMoveDirection = inputDir;

        // Trigger attack if key pressed, not already attacking, and attack object exists
        if (Input.GetKeyDown(attackKey) && !isAttacking && attackObject != null)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Calculate attack hitbox position in front of player
        Vector3 attackPos = transform.position + new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0).normalized * attackRange;

        // Position and rotate the visual sprite for feedback
        Vector3 visualPos = attackPos + new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0).normalized * attackVisualOffset;
        attackObject.transform.position = visualPos;

        // Rotate sprite to match direction
        if (lastMoveDirection == Vector2.right) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (lastMoveDirection == Vector2.left) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
        else if (lastMoveDirection == Vector2.up) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        else if (lastMoveDirection == Vector2.down) attackObject.transform.localRotation = Quaternion.Euler(0, 0, -90);

        // Enable sprite and play animation
        attackSprite.enabled = true;
        attackAnimator.Play("AttackAnimation", -1, 0f);

        // Hit detection using OverlapBox rotated to direction
        Vector2 hitboxSize = new Vector2(width, height);
        float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPos, hitboxSize, angle);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                // If enemy implements IStompable, call Stomped; otherwise destroy
                IStompable stompable = hit.GetComponent<IStompable>();
                if (stompable != null)
                    stompable.Stomped();
                else
                    Destroy(hit.gameObject);
            }
        }

        // Wait for attack duration, then disable visual
        yield return new WaitForSeconds(attackDuration);
        attackSprite.enabled = false;
        isAttacking = false;
    }

    // Draw attack gizmo in editor for debugging
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Vector3 attackPos = transform.position + new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0).normalized * attackRange;
        Gizmos.DrawWireSphere(attackPos, 0.2f);
    }
}