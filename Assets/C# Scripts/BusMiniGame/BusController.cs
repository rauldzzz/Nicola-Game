using UnityEngine;

public class BusController : MonoBehaviour
{
    public float laneDistance = 1.5f; 
    public float switchSpeed = 10f;

    private float targetX;
    private bool isRightLane = false; 

    void Start()
    {
        targetX = -laneDistance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            isRightLane = true;
            targetX = laneDistance;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            isRightLane = false;
            targetX = -laneDistance;
        }

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, switchSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            Debug.Log("Hit a rock! Game Over.");
            Time.timeScale = 0;
        }
    }
}