using UnityEngine;

public class TriggerCutscene : MonoBehaviour
{
    public OverlayCutscene cutsceneController; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cutsceneController.StartCutscene();
            gameObject.SetActive(false); 
        }
    }
}