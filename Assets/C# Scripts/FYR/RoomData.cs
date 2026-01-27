using UnityEngine;

/*
 * RoomData
 * --------
 * ScriptableObject used to define a room type and its prefab for procedural generation.
 * - `type` determines the kind of room (Start, Normal, End, DeadEnd, etc.)
 * - `prefab` is the actual GameObject to instantiate in the scene
 */
[CreateAssetMenu(fileName = "RoomData", menuName = "FYR/RoomData")]
public class RoomData : ScriptableObject
{
    public RoomType type;      // Type of the room (Start, Normal, End, DeadEnd, etc.)
    public GameObject prefab;  // Prefab to instantiate when generating this room
}
