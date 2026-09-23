using UnityEngine;
using UnityEngine.UI;

public class UI_DialogueWindow : UI_ScreenBase
{
    public TMPro.TextMeshProUGUI characterName;
    public TMPro.TextMeshProUGUI characterDialoue;

    [SerializeField] Image image;

    public void SetDialogue(
        string npcName,
        Sprite npcPortrait,
        string dialogue)
    {
        characterName.text = npcName;
        characterDialoue.text = dialogue;

        if (image != null)
        {
            image.sprite = npcPortrait;
        }
    }
}