using UnityEngine;

/*
 * NPCDialogue
 * -----------
 * ScriptableObject for storing NPC dialogue data.
 * - Stores NPC name, portrait, dialogue lines, and typing settings.
 * - Supports automatic line progression for certain lines.
 * - Can optionally include voice audio with pitch adjustment.
 */

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;               
    public Sprite npcPortrait;           
    public string[] dialogueLines;       
    public bool[] autoProgressLines;     
    public float autoProgressDelay = 1.5f; 
    public float typingSpeed = 0.05f;    
    public AudioClip voiceSound;         
    public float voicePitch = 1f;        
}