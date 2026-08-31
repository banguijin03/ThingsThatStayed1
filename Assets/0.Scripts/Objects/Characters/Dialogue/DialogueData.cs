using System;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string nextDialogueID;
}

[Serializable]
public class Dialogue
{
    public string dialogueID;

    [TextArea]
    public string[] dialogues;

    public string[] conditions;

    public int maxCount;

    public bool isRandom;

    public int priority;

    public string nextDialogueID;

    public DialogueChoice[] choices;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string npcID;
    public string npcName;

    // 대화창에 표시할 NPC 이미지
    public Sprite npcPortrait;

    public Dialogue[] dialogueList;
}