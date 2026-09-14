using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : ManagerBase
{
    Dialogue currentDialogue;
    DialogueData currentDialogueData;

    int currentIndex;
    bool isDialogue;

    // 현재 활성화된 조건
    HashSet<string> conditions = new HashSet<string>();

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

        if (data.dialogueList == null || data.dialogueList.Length == 0)
            return;

        currentDialogueData = data;

        // 어떤 대화를 사용할지 선택
        currentDialogue = SelectDialogue(data);

        if (currentDialogue == null)
            return;

        currentIndex = 0;
        isDialogue = true;

        UIManager.ClaimOpenUI(UIType.Dialogue);

        ShowDialogue();
    }

    Dialogue SelectDialogue(DialogueData data)
    {
        Dialogue selectedDialogue = null;

        List<Dialogue> randomDialogues = new List<Dialogue>();

        foreach (Dialogue dialogue in data.dialogueList)
        {
            // 랜덤으로 선택되는 일반 대화
            if (dialogue.isRandom)
            {
                randomDialogues.Add(dialogue);
                continue;
            }

            // 특별 대화의 조건 확인
            if (!CheckConditions(dialogue))
                continue;

            // 조건을 만족하는 대화 중 우선순위가 높은 대화 선택
            if (selectedDialogue == null ||
                dialogue.priority > selectedDialogue.priority)
            {
                selectedDialogue = dialogue;
            }
        }

        // 특별 대화가 있으면 특별 대화 우선
        if (selectedDialogue != null)
        {
            return selectedDialogue;
        }

        // 특별 대화가 없다면 일반 대화 중 하나 랜덤 선택
        if (randomDialogues.Count > 0)
        {
            int randomIndex = Random.Range(0, randomDialogues.Count);
            return randomDialogues[randomIndex];
        }

        return null;
    }

    bool CheckConditions(Dialogue dialogue)
    {
        // 조건이 없으면 특별 대화로 선택하지 않음
        if (dialogue.conditions == null ||
            dialogue.conditions.Length == 0)
        {
            return false;
        }

        foreach (string condition in dialogue.conditions)
        {
            if (!conditions.Contains(condition))
                return false;
        }

        return true;
    }

    public void SetCondition(string condition)
    {
        if (string.IsNullOrEmpty(condition))
            return;

        conditions.Add(condition);
    }

    public void RemoveCondition(string condition)
    {
        conditions.Remove(condition);
    }

    public bool HasCondition(string condition)
    {
        return conditions.Contains(condition);
    }

    public void NextDialogue()
    {
        if (!isDialogue) return;
        if (currentDialogue == null) return;

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
        UI_DialogueWindow dialogueWindow =
            UIManager.ClaimGetUI(UIType.Dialogue) as UI_DialogueWindow;

        if (dialogueWindow == null) return;

        Sprite portrait = currentDialogue.npcPortrait;

        // 개별 이미지가 없으면 기본 이미지 사용
        if (portrait == null)
        {
            portrait = currentDialogueData.defaultPortrait;
        }

        dialogueWindow.SetDialogue(
            currentDialogueData.npcName,
            portrait,
            currentDialogue.dialogues[currentIndex]
        );
    }

    public void EndDialogue()
    {
        UIManager.ClaimCloseUI(UIType.Dialogue);

        currentDialogueData = null;
        currentDialogue = null;
        currentIndex = 0;
        isDialogue = false;
    }
}