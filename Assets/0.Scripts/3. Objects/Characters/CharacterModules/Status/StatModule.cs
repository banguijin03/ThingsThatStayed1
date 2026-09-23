using System;
using System.Collections;
using UnityEngine;

public class StatModule : CharacterModule
{
    public event Action OnDead;

    public FillValue HP = new FillValue(100, 100);
    public FillValue Hunger = new FillValue(100, 100);
    public FillValue Thirst = new FillValue(100, 100);
    public FillValue Feeling = new FillValue(100, 100);

    public bool IsDead => HP.Current <= 0;
    public bool IsHungry => Hunger.Current <= 0;
    public bool IsThirst => Thirst.Current <= 0;
    public bool IsPanic => Feeling.Current <= 0;

    public AbnormalCondition IsPoison;  //
    public AbnormalCondition IsRunning; //
    public AbnormalCondition IsBurn;    //
    public AbnormalCondition IsCold;    //

    bool deadTriggered = false;

    void Start()
    {
        StartCoroutine(StatusRoutine());
    }

    IEnumerator StatusRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (IsDead)
            {
                if (!deadTriggered)
                {
                    deadTriggered = true;
                    OnDead?.Invoke();
                }
                yield break;
            }

            Hunger.DecreaseCurrent(20);
            Thirst.DecreaseCurrent(17);

            if (IsHungry)
            {
                HP.DecreaseCurrent(20);
            }

            if (IsThirst)
            {
                HP.DecreaseCurrent(20);
            }

            if (IsPoison)
            {
                HP.DecreaseCurrent(1);
            }

            if (IsRunning)
            {
                Hunger.DecreaseCurrent(1);
                Thirst.DecreaseCurrent(1);
            }

            if (IsBurn)
            {
                HP.DecreaseCurrent(1);
            }

            if (IsCold)
            {
                HP.DecreaseCurrent(1);
            }
        }
    }
} 