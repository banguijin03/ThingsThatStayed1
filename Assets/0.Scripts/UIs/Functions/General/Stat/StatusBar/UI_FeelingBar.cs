using UnityEngine;
using UnityEngine.UI;

public class UI_FeelingBar : UIBase
{
    [SerializeField] Slider feelingBar;
    [SerializeField] CharacterBase targetCharacter;

    StatModule statModule;

    void Start()
    {
        feelingBar.minValue = 0f;
        feelingBar.maxValue = 1f;
        feelingBar.interactable = false;

        statModule = targetCharacter.GetComponent<StatModule>();

        if (statModule != null)
        {
            statModule.Feeling.OnValueChanged += RefreshFeelingBar;

            RefreshFeelingBar(statModule.Feeling.Current, statModule.Feeling.Max);
        }
    }

    void OnDestroy()
    {
        if (statModule != null)
        {
            statModule.Feeling.OnValueChanged -= RefreshFeelingBar;
        }
    }

    void RefreshFeelingBar(int current, int max)
    {
        feelingBar.value = (float)current / max;
    }
}
