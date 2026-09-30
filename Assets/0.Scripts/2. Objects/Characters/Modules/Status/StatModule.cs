using System;
using UnityEngine;

public class StatModule : CharacterModule
{
    [Header("기본 스탯")]
    [SerializeField] int maxHP = 100;
    [SerializeField] int maxHunger = 100;
    [SerializeField] int maxThirst = 100;
    [SerializeField] int maxStability = 100;

    [Header("감소 설정")]
    [SerializeField] float decreaseInterval = 10f;
    [SerializeField] int hungerDecrease = 1;
    [SerializeField] int thirstDecrease = 1;
    [SerializeField] int stabilityDecrease = 1;

    public sealed override Type RegistrationType => typeof(StatModule);

    public StatValue HP { get; private set; }
    public StatValue Hunger { get; private set; }
    public StatValue Thirst { get; private set; }
    public StatValue Stability { get; private set; }

    float timer;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        HP = new StatValue(maxHP, maxHP);
        Hunger = new StatValue(maxHunger, maxHunger);
        Thirst = new StatValue(maxThirst, maxThirst);
        Stability = new StatValue(maxStability, maxStability);

        timer = 0f;

        GameManager.OnUpdateCharacter -= UpdateStat;
        GameManager.OnUpdateCharacter += UpdateStat;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        GameManager.OnUpdateCharacter -= UpdateStat;

        HP = null;
        Hunger = null;
        Thirst = null;
        Stability = null;

        base.OnUnregistration(oldOwner);
    }

    void UpdateStat(float deltaTime)
    {
        timer += deltaTime;

        if (timer < decreaseInterval) return;

        timer -= decreaseInterval;

        Hunger.Decrease(hungerDecrease);
        Thirst.Decrease(thirstDecrease);
        Stability.Decrease(stabilityDecrease);

        Debug.Log($"Hunger: {Hunger.Current}, Thirst: {Thirst.Current}, Stability: {Stability.Current}");
    }
}