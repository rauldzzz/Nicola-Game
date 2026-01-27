/*
 * RoomType
 * --------
 * Enum representing the type of a room in the procedural map.
 * - Start: The starting room where the player begins
 * - Normal: Standard room with possible connections
 * - DeadEnd: A room that ends without further connections
 * - EndUp/EndDown/EndLeft/EndRight: End rooms with a specific exit direction
 */
public enum RoomType
{
    Start,
    Normal,
    DeadEnd,
    EndUp,
    EndDown,
    EndLeft,
    EndRight
}
