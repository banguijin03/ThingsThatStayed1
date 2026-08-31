using UnityEngine;

public class NPCInteractionModule : NPCModule
{
    [SerializeField] PlayerController playerCharacter;
    [SerializeField] NPCDialogueModule Dialogue;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        playerCharacter = FindAnyObjectByType<PlayerController>(
            FindObjectsInactive.Include
        );

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

        // 이미 대화 중이라면
        // 다음 대사로 넘어감
        if (GameManager.Instance.Dialogue.IsDialogue)
        {
            GameManager.Instance.Dialogue.NextDialogue();
            return;
        }

        // 대화 중이 아니라면
        // NPC와 상호작용 가능한지 확인
        Vector2 npcPosition = Owner.transform.position;
        Vector2 playerPosition = playerCharacter.Character.transform.position;

        // NPC와 플레이어의 거리 확인
        float distance = Vector2.Distance(npcPosition, playerPosition);

        if (distance > 2)
        {
            return;
        }

        // 플레이어가 NPC를 바라보고 있는 방향 확인
        Vector2 directionToNPC =
            (npcPosition - playerPosition).normalized;

        Vector2 playerDirection =
            playerCharacter.Character.LookRotation.normalized;

        float dot = Vector2.Dot(playerDirection, directionToNPC);

        // 플레이어가 NPC를 바라보고 있다면 대화 시작
        if (dot > 0.7)
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