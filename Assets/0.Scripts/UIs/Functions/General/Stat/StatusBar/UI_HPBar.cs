using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UIBase
{
    [SerializeField] Slider hpBar;
    [SerializeField] CharacterBase targetCharacter;

    StatModule statModule;

    void Start()
    {
        if (targetCharacter == null)
            targetCharacter = FindAnyObjectByType<CharacterBase>();

        hpBar.minValue = 0f;
        hpBar.maxValue = 1f;
        hpBar.interactable = false;

        statModule= targetCharacter.GetModule<StatModule>();

        if (statModule != null)
        {
            statModule.HP.OnValueChanged += RefreshHPBar;
            RefreshHPBar(statModule.HP.Current, statModule.HP.Max);
        }
    }

    void OnDestroy()
    {
        if (statModule != null)
        {
            statModule.HP.OnValueChanged -= RefreshHPBar;
        }
    }

    void RefreshHPBar(int current, int max)
    {
        hpBar.value = (float)current / max;
    }
}