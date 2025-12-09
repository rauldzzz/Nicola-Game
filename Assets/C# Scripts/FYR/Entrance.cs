using UnityEngine;

public class Entrance : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }
    public Direction entranceDirection;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
}
