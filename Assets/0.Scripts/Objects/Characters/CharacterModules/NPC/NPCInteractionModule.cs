using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class NPCInteractionModule : NPCModule
{
    [SerializeField] PlayerController playerCharacter;
    [SerializeField] NPCDialogueModule Dialogue;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        playerCharacter = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        //다이어로그를 어떤 모듈에서 불러올지
        Dialogue = newOwner.GetModule<NPCDialogueModule>();

        InputManager.OnInteraction -= InteractionNPC;
        InputManager.OnInteraction += InteractionNPC;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        InputManager.OnInteraction -= InteractionNPC;
        base.OnUnregistration(oldOwner);
    }

    public void InteractionNPC(bool value)
    {
        if (!value)  return;
        if (playerCharacter == null)  return;

        if (!Owner) return;

        Vector2 npcPosition = Owner.transform.position;
        Vector2 playerPosition = playerCharacter.Character.transform.position;
        float distance = Vector2.Distance(npcPosition, playerPosition);
        if (distance > 2)
        {
            return;
        }

        //플레이어가 npc를 바라보고 있는 방향이 옳은지 체크
        Vector2 directionToNPC = (npcPosition - playerPosition).normalized;
        Vector2 playerDirection = playerCharacter.Character.LookRotation.normalized;
        float dot = Vector2.Dot(playerDirection, directionToNPC);

        if (dot > 0.7)
        {
            Debug.Log("대화는 준비중이야!");
        }
    }
    public void InteractionObject(bool value)
    {
        //현재 들고있는것이 무엇인지 어떤 오브젝트를 클릭했는지 거리가 어느정도인지에 따라 실행할것
    }

    public void CommendStart()
    {

    }
    public void CommendEnd()
    {

    }
}
