using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "FYR/RoomData")]
public class RoomData : ScriptableObject
{
    public RoomType type;
    public GameObject prefab;
}
