using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class NPCInteractionModule : NPCModule
{
    [SerializeField] PlayerController playerCharacter;
    NPCDialogueModule Dialogue;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        playerCharacter= FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        InputManager.OnInteraction -= Interactionable;
        InputManager.OnInteraction += Interactionable;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        InputManager.OnInteraction -= Interactionable;
        base.OnUnregistration(oldOwner);
    }

    public void Interactionable(bool value)
    {
        Debug.Log("함수는 실행됐어용");
        if (!value)
        {
            Debug.Log("입력을 안하셨는데용?");
            return;
        }
        if (playerCharacter == null)
        {
            Debug.Log("캐릭터가 없는데용?");
            return;
        }
        if (!Owner)
        {
            Debug.Log("주인장이 없는데용?");
            return;
        }

        Vector2 npcPosition = Owner.transform.position;
        Vector2 playerPosition = playerCharacter.Character.transform.position;
        float distance = Vector2.Distance(npcPosition, playerPosition);

        if (distance > 5)
        {
            Debug.Log("너무멀어");
            return;
        }

        //플레이어가 npc를 바라보고 있는 방향이 옳은지 체크
        Vector2 directionToNPC = (npcPosition - playerPosition).normalized;
        Vector2 playerDirection = playerCharacter.Character.LookRotation.normalized;
        float dot = Vector2.Dot(playerDirection, directionToNPC);
        if (dot > 0.7)
        {
            Debug.Log("성공!");
            CommandStart();
        }
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
