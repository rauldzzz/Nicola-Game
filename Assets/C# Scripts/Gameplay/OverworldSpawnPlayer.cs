using UnityEngine;

public class OverworldSpawnPlayer : MonoBehaviour
{
    public Transform defaultSpawnPoint;

    void Start()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.overworldPlayerPosition == Vector3.zero)
        {
            transform.position = defaultSpawnPoint.position;
        }
        else
        {
            transform.position = SaveManager.Instance.overworldPlayerPosition;
        }
    }
}
