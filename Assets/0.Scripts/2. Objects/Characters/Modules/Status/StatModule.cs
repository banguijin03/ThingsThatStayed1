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

    public event Action<int, int> OnHPChanged;

    float timer;
    float hpTimer;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        HP        = new StatValue(maxHP, maxHP);
        Hunger    = new StatValue(maxHunger, maxHunger);
        Thirst    = new StatValue(maxThirst, maxThirst);
        Stability = new StatValue(maxStability, maxStability);

        timer     = 0f;
        hpTimer   = 0f;

        GameManager.OnUpdateCharacter -= UpdateStat;
        GameManager.OnUpdateCharacter += UpdateStat;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        GameManager.OnUpdateCharacter -= UpdateStat;

        HP          = null;
        Hunger      = null;
        Thirst      = null;
        Stability   = null;

        base.OnUnregistration(oldOwner);
    }

    void UpdateStat(float deltaTime)
    {
        timer += deltaTime;
        hpTimer += deltaTime;

        if (timer >= decreaseInterval)
        {
            timer -= decreaseInterval;

            bool hungerWasEmpty = Hunger.IsEmpty;
            bool thirstWasEmpty = Thirst.IsEmpty;

            Hunger.Decrease(hungerDecrease);
            Thirst.Decrease(thirstDecrease);
            Stability.Decrease(stabilityDecrease);

            bool hungerBecameEmpty = !hungerWasEmpty && Hunger.IsEmpty;
            bool thirstBecameEmpty = !thirstWasEmpty && Thirst.IsEmpty;

            if (hungerBecameEmpty) DecreaseHP(1);
            if (thirstBecameEmpty) DecreaseHP(1);
            if (hungerBecameEmpty || thirstBecameEmpty) hpTimer = 0f;
        }

        if (hpTimer >= 1f)
        {
            hpTimer -= 1f;
            if (Hunger.IsEmpty) DecreaseHP(1);
            if (Thirst.IsEmpty) DecreaseHP(1);
        }
    }

    void DecreaseHP(int amount)
    {
        int previousHP = HP.Current;

        HP.Decrease(amount);

        int currentHP = HP.Current;

        if (previousHP != currentHP)
        {
            OnHPChanged?.Invoke(previousHP, currentHP);
        }
    }

    public bool IncreaseHP(int amount)
    {
        if (amount <= 0) return false;
        if (HP == null) return false;
        if (HP.IsMax) return false;

        int previousHP = HP.Current;

        HP.Increase(amount);

        int currentHP = HP.Current;

        if (previousHP == currentHP)
            return false;

        OnHPChanged?.Invoke(previousHP, currentHP);

        return true;
    }

    public bool IncreaseHunger(int amount)
    {
        if (amount <= 0) return false;
        if (Hunger == null) return false;
        if (Hunger.IsMax) return false;

        int previous = Hunger.Current;

        Hunger.Increase(amount);

        return previous != Hunger.Current;
    }

    public bool IncreaseThirst(int amount)
    {
        if (amount <= 0) return false;
        if (Thirst == null) return false;
        if (Thirst.IsMax) return false;

        int previous = Thirst.Current;

        Thirst.Increase(amount);

        return previous != Thirst.Current;
    }

    public bool IncreaseStability(int amount)
    {
        if (amount <= 0) return false;
        if (Stability == null) return false;
        if (Stability.IsMax) return false;

        int previous = Stability.Current;

        Stability.Increase(amount);

        return previous != Stability.Current;
    }
}