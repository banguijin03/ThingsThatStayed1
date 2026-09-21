using UnityEngine;

public class NPCInteractionModule : NPCModule
{
    [SerializeField] PlayerController playerCharacter;
    [SerializeField] NPCDialogueModule Dialogue;
    [SerializeField] InteractionCondition interactionCondition;

    bool interactionLock;

    // 현재 대화를 시작한 NPC
    static NPCInteractionModule currentDialogueNPC;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        playerCharacter = FindAnyObjectByType<PlayerController>(
            FindObjectsInactive.Include
        );

        // 현재 NPC의 DialogueModule
        Dialogue = newOwner.GetComponent<NPCDialogueModule>();

        // 현재 NPC의 InteractionCondition
        interactionCondition =
            newOwner.GetComponent<InteractionCondition>();

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
        // F키를 뗐을 때
        if (!value)
        {
            interactionLock = false;
            return;
        }

        if (playerCharacter == null) return;
        if (!Owner) return;
        if (Dialogue == null) return;

        // 이미 대화 중
        if (GameManager.Instance.Dialogue.IsDialogue)
        {
            // 현재 대화를 시작한 NPC만 다음 대사를 진행
            if (currentDialogueNPC != this)
                return;

            GameManager.Instance.Dialogue.NextDialogue();

            return;
        }

        // 대화 종료 직후
        // 같은 F 입력으로 재시작 방지
        if (interactionLock)
            return;

        if (interactionCondition == null)
            return;

        // 거리 + 바라보는 방향 확인
        if (!interactionCondition.CanInteract())
            return;

        // 이번 F 입력으로 대화 시작
        interactionLock = true;

        // 현재 대화를 시작한 NPC로 등록
        currentDialogueNPC = this;

        Dialogue.StartDialogue();
    }

    public void InteractionObject(bool value)
    {
        // 오브젝트 상호작용
    }

    public void CommendStart()
    {

    }

    public void CommendEnd()
    {

    }
}