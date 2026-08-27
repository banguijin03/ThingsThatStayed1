using System.Collections;
using UnityEngine;

public class DialogueManager : ManagerBase
{
    DialogueData currentDialogue;
    int currentIndex;
    bool isDialogue;

    public bool IsDialogue => isDialogue;

    protected override IEnumerator OnConnected(GameManager gameManager)
    {
        yield break;
    }

    protected override void OnDisconnected()
    {
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null) return;
        if (isDialogue) return;

        currentDialogue = data;
        currentIndex = 0;
        isDialogue = true;

        ShowDialogue();
    }

    public void NextDialogue()
    {
        if (!isDialogue) return;

        currentIndex++;

        if (currentIndex >= currentDialogue.dialogues.Length)
        {
            EndDialogue();
            return;
        }

        ShowDialogue();
    }

    void ShowDialogue()
    {
        Debug.Log(currentDialogue.dialogues[currentIndex]);
    }

    public void EndDialogue()
    {
        currentDialogue = null;
        currentIndex = 0;
        isDialogue = false;
    }
}