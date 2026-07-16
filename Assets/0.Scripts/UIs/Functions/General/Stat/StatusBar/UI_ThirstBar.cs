using UnityEngine;
using UnityEngine.UI;

public class UI_ThirstBar : UIBase
{
    [SerializeField] Slider thirstBar;
    [SerializeField] CharacterBase targetCharacter;

    StatModule statModule;

    void Start()
    {
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
        thirstBar.value = (float)current / max;
    }
}
