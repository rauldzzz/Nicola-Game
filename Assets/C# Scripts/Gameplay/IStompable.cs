using UnityEngine;

/*
 * IStompable
 * ----------
 * Interface for objects that can be stomped by the player.
 * - Any class implementing this must define a Stomped() method.
 * - Used to handle stomp interactions consistently across different enemies.
 */

// Interface for enemies that can be stomped by the player
public interface IStompable
{
    void Stomped();
}