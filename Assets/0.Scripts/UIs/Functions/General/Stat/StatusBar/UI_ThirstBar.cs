using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_ThirstBar : UIBase
{
    [SerializeField] Slider thirstBar;
    [SerializeField] CharacterBase targetCharacter;
    UI_MovableScreen stop;
    StatModule statModule;

    void Start()
    {
        if (targetCharacter == null)
            targetCharacter = FindAnyObjectByType<CharacterBase>();

        thirstBar.minValue = 0f;
        thirstBar.maxValue = 1f;
        thirstBar.interactable = false;

        statModule = targetCharacter.GetComponent<StatModule>();

        if (statModule != null)
        {
            statModule.Thirst.OnValueChanged += RefreshThirstBar;

            RefreshThirstBar(statModule.Thirst.Current, statModule.Thirst.Max);
        }
    }

    void OnDestroy()
    {
        if (statModule != null)
        {
            statModule.Thirst.OnValueChanged -= RefreshThirstBar;
        }
    }

    void RefreshThirstBar(int current, int max)
    {
        if (stop.stop = false) thirstBar.value = (float)current / max;
    }
}
