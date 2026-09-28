using UnityEngine;

public class NPCInteractionModule : NPCModule
{
    [SerializeField] PlayerController playerCharacter;
    [SerializeField] NPCDialogueModule Dialogue;
    [SerializeField] InteractionCondition interactionCondition;

    bool interactionLock;
    static NPCInteractionModule currentDialogueNPC;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        playerCharacter = GameManager.Instance.Player.GetComponent<PlayerController>();
        Dialogue = newOwner.GetComponent<NPCDialogueModule>();
        interactionCondition = newOwner.GetComponent<InteractionCondition>();

        InputManager.OnInteraction -= InteractionNPC;
        InputManager.OnInteraction += InteractionNPC;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        InputManager.OnInteraction -= InteractionNPC;

        if (currentDialogueNPC == this)
        {
            currentDialogueNPC = null;
        }

        UIManager.ClaimCloseUI(UIType.Dialogue);

        base.OnUnregistration(oldOwner);
    }

    public void InteractionNPC(bool value)
    {
        if (!value)
        {
            interactionLock = false;
            return;
        }

        if (playerCharacter == null) return;
        if (!Owner) return;
        if (Dialogue == null) return;

        if (GameManager.Instance.Dialogue.IsDialogue)
        {
            if (currentDialogueNPC != this)
                return;

            GameManager.Instance.Dialogue.NextDialogue();
            return;
        }

        if (interactionLock) return;
        if (interactionCondition == null) return;
        if (!interactionCondition.CanInteract()) return;

        interactionLock = true;
        currentDialogueNPC = this;

        Dialogue.StartDialogue();
    }

    public void InteractionObject(bool value)
    {
    }

    public void CommendStart()
    {
    }

    public void CommendEnd()
    {
    }
}