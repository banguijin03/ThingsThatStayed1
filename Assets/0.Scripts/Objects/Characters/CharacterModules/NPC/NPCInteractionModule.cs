using UnityEngine;

public class NPCInteractionModule : NPCModule
{
    [SerializeField] PlayerController playerCharacter;
    [SerializeField] NPCDialogueModule Dialogue;
    bool interactionLock;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        playerCharacter = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);

        // 현재 NPC의 DialogueModule 가져오기
        Dialogue = newOwner.GetComponent<NPCDialogueModule>();

        InputManager.OnInteraction -= InteractionNPC;
        InputManager.OnInteraction += InteractionNPC;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        InputManager.OnInteraction -= InteractionNPC;

        UIManager.ClaimCloseUI(UIType.Dialogue);

        base.OnUnregistration(oldOwner);
    }

    public void InteractionNPC(bool value)
    {
        if (!value) return;
        if (playerCharacter == null) return;
        if (!Owner) return;

        Vector2 npcPosition = Owner.transform.position;
        Vector2 playerPosition =
            playerCharacter.Character.transform.position;

        // 거리 확인
        float distance = Vector2.Distance(
            npcPosition,
            playerPosition
        );


        // 멀리 있으면 여기서 끝
        if (distance > 2f) return; 

        // 가까운 NPC만 대화 상태 확인
        if (GameManager.Instance.Dialogue.IsDialogue)
        {
            GameManager.Instance.Dialogue.NextDialogue();
            return;
        }

        // 바라보는 방향 확인
        Vector2 directionToNPC =
            (npcPosition - playerPosition).normalized;

        Vector2 playerDirection =
            playerCharacter.Character.LookRotation.normalized;

        float dot = Vector2.Dot(
            playerDirection,
            directionToNPC
        ); 

        if (dot > 0.5f)
        {
            Dialogue.StartDialogue();
        }
    }

    public void InteractionObject(bool value)
    {
        // 현재 들고 있는 것이 무엇인지,
        // 어떤 오브젝트를 클릭했는지,
        // 거리가 어느 정도인지에 따라 실행할 것
    }

    public void CommendStart()
    {

    }

    public void CommendEnd()
    {

    }
}