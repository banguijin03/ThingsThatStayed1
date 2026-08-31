using UnityEngine;

public class NPCDialogueModule : NPCModule
{
    [SerializeField] DialogueData dialogueData;

    public void StartDialogue()
    {
        GameManager.Instance.Dialogue.StartDialogue(dialogueData);
    }
}