using UnityEngine;

public class UI_HungerWarning : UIBase
{
    [SerializeField] CharacterBase targetCharacter;

    StatModule stat;

    public GameObject warningUI;

    private void Start()
    {
        stat = targetCharacter.GetModule<StatModule>();
    }

    private void Check()
    {
        warningUI.SetActive(stat.IsHungry);
    }

    private void Update()
    {
        Check();
    }
}