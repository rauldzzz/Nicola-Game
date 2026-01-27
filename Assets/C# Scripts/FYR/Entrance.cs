using UnityEngine;

/*
 * Entrance
 * --------
 * Represents a single doorway/entrance in a room.
 * - Can be open or closed
 * - Has a direction (Up, Down, Left, Right)
 */
public class Entrance : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right } // Orientation of the entrance
    public Direction entranceDirection;            // The direction this entrance faces

    // Enables the entrance
    public void Open() => gameObject.SetActive(true);

    // Disables the entrance
    public void Close() => gameObject.SetActive(false);
}