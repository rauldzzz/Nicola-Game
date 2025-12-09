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
    private Vector3 lastMoveDirection = Vector3.down;

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
        if (!movementScript.IsMoving)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) lastMoveDirection = Vector3.up;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) lastMoveDirection = Vector3.down;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) lastMoveDirection = Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) lastMoveDirection = Vector3.right;
        }

        if (Input.GetKeyDown(attackKey) && !isAttacking && attackObject != null)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Calculate attack position
        Vector3 attackPos = transform.position + lastMoveDirection.normalized * attackRange;

        // Move and rotate the attackObject to match direction
        Vector3 visualPos = attackPos + lastMoveDirection.normalized * attackVisualOffset;
        attackObject.transform.position = visualPos;

        if (lastMoveDirection == Vector3.right) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (lastMoveDirection == Vector3.left) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
        else if (lastMoveDirection == Vector3.up) attackObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        else if (lastMoveDirection == Vector3.down) attackObject.transform.localRotation = Quaternion.Euler(0, 0, -90);

        // Enable attack object and play animation
        attackSprite.enabled = true;
        attackAnimator.Play("AttackAnimation", -1, 0f);

        // Define hitbox size (adjust width/height as needed)
        Vector2 hitboxSize = new Vector2(width, height);

        // Calculate rotation angle for OverlapBox
        float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;

        // Detect enemies in attack hitbox
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

        // Wait for attack animation / duration
        yield return new WaitForSeconds(attackDuration);

        attackSprite.enabled = false;
        isAttacking = false;

    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Vector3 attackPos = transform.position + lastMoveDirection.normalized * attackRange;
        Gizmos.DrawWireSphere(attackPos, 0.2f);
    }

}
