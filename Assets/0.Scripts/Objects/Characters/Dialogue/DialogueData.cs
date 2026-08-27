using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public string[] dialogues;
}
//NPC ID — 어떤 NPC의 대화인지
//대사 내용 — 실제 대사
//대화 ID — 각각의 대화를 구분하기 위한 ID
//대화 등장 조건 — 퀘스트 상태, 특정 아이템 보유 등
//등장 횟수 제한 — 몇 번까지 보여줄지
//랜덤 여부 — 여러 대사 중 랜덤으로 선택할지
//대화 우선순위 — 여러 조건이 동시에 만족될 때 어떤 대화를 우선할지
//다음 대화 ID — 특정 대화 이후 어떤 대화로 이어질지
//선택지 정보 — 선택지가 있는 대화라면 선택지 내용과 연결될 대화 ID