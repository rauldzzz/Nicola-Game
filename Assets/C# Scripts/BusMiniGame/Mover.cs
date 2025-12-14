using UnityEngine;

public class Mover : MonoBehaviour
{

    void Update()
    {
        float currentSpeed = BusGameManager.Instance.gameSpeed;

        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}