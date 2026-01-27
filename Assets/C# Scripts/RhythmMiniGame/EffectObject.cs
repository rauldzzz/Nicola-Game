using UnityEngine;

/*
 * EffectObject
 * ------------
 * Simple object that destroys itself after a set lifetime.
 * Can be used for particle effects, visual feedback, or temporary objects.
 */

public class EffectObject : MonoBehaviour
{
    public float lifetime = 1.0f; // How long before this object is destroyed

    void Update()
    {
        // Destroy the object after its lifetime has passed
        Destroy(gameObject, lifetime);
    }
}