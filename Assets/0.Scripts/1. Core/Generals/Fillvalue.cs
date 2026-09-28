using System;
using UnityEngine;

public class FillValue
{
    public event Action<int, int> OnValueChanged;

    int _current, _min, _max;

    public int Current
    {
        get => _current;

        set
        {
            _current = Mathf.Clamp(value, Min, Max);

            OnValueChanged?.Invoke(_current, _max);
        }
    }

    public int Min => _min;
    public int Max => _max;
    public float Percent => (float)Current / Max;

    public bool IsEmpty => _current <= Min;
    public bool IsMax => _current >= Max;
    public bool IsUnderZero => _current <= 0;

    public FillValue(int current, int max, int min = 0)
    {
        _max = max;
        _min = min;

        Current = current;
    }

    public int IncreaseCurrent(int value) => Current += value;
    public int DecreaseCurrent(int value) => Current -= value;

    public int SetCurrent(int value) => Current = value;

    public float SetPercent(float value) =>
        Current = Mathf.CeilToInt(
            Mathf.Lerp(Min, Max, Mathf.Clamp(value, 0.0f, 1.0f)));

    public void SetMax(int value)
    {
        _max = value;
        Current = Current;
    }

    public void SetMin(int value)
    {
        _min = value;
        Current = Current;
    }
}