using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_HungerBar : UIBase
{
    [SerializeField] Slider hungerBar;
    [SerializeField] CharacterBase targetCharacter;
    UI_MovableScreen stop;

    StatModule statModule;

    void Start()
    {
        if (targetCharacter == null)
            targetCharacter = FindAnyObjectByType<CharacterBase>();

        hungerBar.minValue = 0f;
        hungerBar.maxValue = 1f;
        hungerBar.interactable = false;

        statModule = targetCharacter.GetComponent<StatModule>();

        if (statModule != null)
        {
            statModule.Hunger.OnValueChanged += RefreshHungerBar;

            RefreshHungerBar(statModule.Hunger.Current, statModule.Hunger.Max);
        }
    }

    void OnDestroy()
    {
        if (statModule != null)
        {
            statModule.Hunger.OnValueChanged -= RefreshHungerBar;
        }
    }

    void RefreshHungerBar(int current, int max)
    {
        if (stop.stop = false) hungerBar.value = (float)current / max;
    }
}
