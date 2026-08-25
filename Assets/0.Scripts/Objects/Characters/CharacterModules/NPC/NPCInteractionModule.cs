using UnityEngine;

public class NPCInteractionModule : NPCModule
{
    bool able = true;
    NPCDialogueModule Dialogue;
    public void PlayerInteration()
    {
        InputManager.OnInteraction -=  Interactionable;
        InputManager.OnInteraction +=  Interactionable;
        CommandStart();
    }

    public void Interactionable(bool value)
    {
        if (!value) return;
    }

    public void CommandStart()
    {
        Dialogue.Dialoogue();
        CommandEnd();
    }
    public void CommandEnd()
    {
        InputManager.OnInteraction -= Interactionable;
    }
}
