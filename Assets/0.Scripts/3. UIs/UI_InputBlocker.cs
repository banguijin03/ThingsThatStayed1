using System;
using UnityEngine;

[Flags]
public enum InputType
{
    None = 0,
    Move = 1 << 0,
    Interaction = 1 << 1,
    Shift = 1 << 2,
    Roll = 1 << 3,
    Inventory = 1 << 4,
    Cancel = 1 << 5,
    Mouse = 1 << 6,
}

public class UI_InputBlocker : MonoBehaviour
{
    [SerializeField]
    InputType allowedInput = InputType.None;

    public InputType AllowedInput => allowedInput;

    public bool IsAllowed(InputType inputType)
    {
        return (allowedInput & inputType) != 0;
    }
}