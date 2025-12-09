using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room Data")]
    public RoomData startRoom;
    public RoomData[] endRooms;             // 4 EndRoom Varianten
    public RoomData[] normalRooms;          // Room1 - Room6
    public RoomData[] deadEndRooms;         // Room7 - Room10

    [Header("Dungeon Settings")]
    public int minDistanceStartToEnd = 5;
    public int maxRooms = 30;

    private Dictionary<Vector2Int, GameObject> placedRooms = new();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // 1. Set StartRoom
        Vector2Int currentPos = Vector2Int.zero;
        GameObject startObj = Instantiate(startRoom.prefab, Vector3.zero, Quaternion.identity);
        placedRooms[currentPos] = startObj;

        // List possible directions
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // 2. Generate Mainpath  (Start → End)
        List<Vector2Int> path = new List<Vector2Int> { currentPos };

        while (path.Count < minDistanceStartToEnd)
        {
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];
            Vector2Int nextPos = currentPos + dir;

            if (placedRooms.ContainsKey(nextPos)) continue;

            // Normal Room
            RoomData randomRoom = normalRooms[Random.Range(0, normalRooms.Length)];
            Vector3 pos = new Vector3(nextPos.x, nextPos.y, 0) * 20f;
            GameObject r = Instantiate(randomRoom.prefab, pos, Quaternion.identity);
            placedRooms[nextPos] = r;

            path.Add(nextPos);
            currentPos = nextPos;
        }

        // 3. Choose EndRoom
        RoomData chosenEnd = endRooms[Random.Range(0, endRooms.Length)];
        Vector2Int endPos = currentPos + dirs[Random.Range(0, dirs.Length)];
        Vector3 epos = new Vector3(endPos.x, endPos.y, 0) * 20f;
        GameObject endObj = Instantiate(chosenEnd.prefab, epos, Quaternion.identity);
        placedRooms[endPos] = endObj;

        // 4. Optional: Set Deadends at random Rooms
        foreach (var pos in path)
        {
            if (Random.value < 0.4f)
            {
                Vector2Int deadDir = dirs[Random.Range(0, dirs.Length)];
                Vector2Int dp = pos + deadDir;

                if (placedRooms.ContainsKey(dp)) continue;

                RoomData deadRoom = deadEndRooms[Random.Range(0, deadEndRooms.Length)];
                Vector3 dpos = new Vector3(dp.x, dp.y, 0) * 20f;
                Instantiate(deadRoom.prefab, dpos, Quaternion.identity);
                placedRooms[dp] = deadRoom.prefab;
            }
        }

        Debug.Log("Map Generated!");
    }
}
