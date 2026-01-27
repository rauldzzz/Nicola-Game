using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/*
 * NPC
 * ---
 * Implements IInteractable to allow the player to interact with NPCs.
 * - Displays dialogue with typing effect.
 * - Supports multi-line dialogue and auto-progressing lines.
 * - Prevents interaction while dialogue is active.
 * - Invokes an event when the dialogue finishes for external systems.
 */

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Settings")]
    public NPCDialogue dialogueData;            // ScriptableObject containing NPC dialogue
    public GameObject dialoguePanel;            // UI panel for dialogue display
    public TMP_Text dialogueText, nameText;     // Text fields for dialogue and NPC name
    public Image portraitImage;                 // NPC portrait image

    [Header("Events")]
    public UnityEvent OnDialogueFinished;       // Triggered once when dialogue completes

    private int dialogueIndex;                  // Tracks the current line in the dialogue
    private bool isTyping;                      // Whether a line is currently being typed
    private bool isDialogueActive;              // Whether the dialogue panel is active

    public bool HasFinishedDialogue { get; private set; }

    public bool CanInteract() => !isDialogueActive;

    public void Interact()
    {
        if (dialogueData == null) return;

        if (isDialogueActive)
            NextLine(); // Move to the next line if dialogue is already active
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        // Initialize dialogue UI
        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            // Instantly finish typing the current line
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        // Type out the current line letter by letter
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Automatically progress to the next line if enabled
        if (dialogueData.autoProgressLines.Length > dialogueIndex &&
            dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        if (!HasFinishedDialogue)
        {
            HasFinishedDialogue = true;
            OnDialogueFinished?.Invoke(); // Notify external systems that dialogue finished
            Debug.Log($"[{name}] Dialogue finished, showing object now!");
        }
    }
}