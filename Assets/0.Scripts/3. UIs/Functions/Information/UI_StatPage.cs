using UnityEngine;
using UnityEngine.UI;

public class UI_StatPage : UIBase
{
    [Header("HP")]
    [SerializeField] Image hpImage;
    [SerializeField] Sprite[] hpSprites;

    [Header("Hunger")]
    [SerializeField] Slider hungerSlider;

    [Header("Thirst")]
    [SerializeField] Slider thirstSlider;

    [Header("Stability")]
    [SerializeField] Slider stabilitySlider;

    StatModule statModule;

    public void ConnectStat(StatModule newStatModule)
    {
        DisconnectStat();

        if (newStatModule == null) return;

        statModule = newStatModule;

        ConnectHP(statModule.HP);
        ConnectHunger(statModule.Hunger);
        ConnectThirst(statModule.Thirst);
        ConnectStability(statModule.Stability);
    }

    public void DisconnectStat()
    {
        if (statModule == null) return;

        if (statModule.HP != null) statModule.HP.OnValueChanged -= UpdateHP;

        if (statModule.Hunger != null) statModule.Hunger.OnValueChanged -= UpdateHunger;

        if (statModule.Thirst != null) statModule.Thirst.OnValueChanged -= UpdateThirst;

        if (statModule.Stability != null) statModule.Stability.OnValueChanged -= UpdateStability;

        statModule = null;
    }

    void ConnectHP(StatValue stat)
    {
        if (stat == null) return;

        stat.OnValueChanged -= UpdateHP;
        stat.OnValueChanged += UpdateHP;

        UpdateHP(stat.Current, stat.Max);
    }

    void ConnectHunger(StatValue stat)
    {
        if (stat == null) return;

        stat.OnValueChanged -= UpdateHunger;
        stat.OnValueChanged += UpdateHunger;

        UpdateHunger(stat.Current, stat.Max);
    }

    void ConnectThirst(StatValue stat)
    {
        if (stat == null) return;

        stat.OnValueChanged -= UpdateThirst;
        stat.OnValueChanged += UpdateThirst;

        UpdateThirst(stat.Current, stat.Max);
    }

    void ConnectStability(StatValue stat)
    {
        if (stat == null) return;

        stat.OnValueChanged -= UpdateStability;
        stat.OnValueChanged += UpdateStability;

        UpdateStability(stat.Current, stat.Max);
    }

    void UpdateHP(int current, int max)
    {
        if (hpImage == null || hpSprites == null || hpSprites.Length == 0 || max <= 0) return;

        float percent = (float)current / max;
        int index = Mathf.RoundToInt((1f - percent) * (hpSprites.Length - 1));

        index = Mathf.Clamp(index, 0, hpSprites.Length - 1);
        hpImage.sprite = hpSprites[index];                                                                                     
    }                                                                 
    void UpdateHunger(int current, int max)                     
    {
        UpdateSlider(hungerSlider, current, max);
    }

    void UpdateThirst(int current, int max)
    {
        UpdateSlider(thirstSlider, current, max);
    }

    void UpdateStability(int current, int max)
    {
        UpdateSlider(stabilitySlider, current, max);
    }

    void UpdateSlider(Slider slider, int current, int max)
    {
        if (slider == null) return;

        slider.minValue = 0;
        slider.maxValue = max;
        slider.value = current;
    }

    private void OnDestroy()
    {
        DisconnectStat();
    }
}