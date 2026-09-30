using System;
using UnityEngine;

public class StatValue
{
    public event Action<int, int> OnValueChanged;

    int _current;
    int _min;
    int _max;

    public int Current => _current;
    public int Min => _min;
    public int Max => _max;
    public float Percent => Max <= Min ? 0f : (float)(Current - Min) / (Max - Min);

    public bool IsEmpty => Current <= Min;
    public bool IsMax => Current >= Max;

    public StatValue(int current, int max, int min = 0)
    {
        _min = min;
        _max = Mathf.Max(min, max);
        SetCurrent(current);
    }

    public void Increase(int value)
    {
        if (value <= 0) return;
        SetCurrent(Current + value);
    }

    public void Decrease(int value)
    {
        if (value <= 0) return;
        SetCurrent(Current - value);
    }

    public void SetCurrent(int value)
    {
        int newValue = Mathf.Clamp(value, Min, Max);

        if (_current == newValue) return;

        _current = newValue;
        OnValueChanged?.Invoke(_current, _max);
    }

    public void SetMax(int value)
    {
        _max = Mathf.Max(Min, value);
        SetCurrent(Current);
    }

    public void SetMin(int value)
    {
        _min = Mathf.Min(value, Max);
        SetCurrent(Current);
    }
}