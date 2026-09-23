using System;
using UnityEngine;

public class PlayerController : ControllerBase
{
    protected override void OnPossess(CharacterBase newCharacter)
    {
        base.OnPossess(newCharacter);
        InputManager.OnMove -= MoveToDirection;
        InputManager.OnMove += MoveToDirection;
        InputManager.OnMouseLeftButton -= UseTool;
        InputManager.OnMouseLeftButton += UseTool;
    }


    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        base.OnUnpossess(oldCharacter);
        InputManager.OnMove -= MoveToDirection;
        InputManager.OnMouseLeftButton -= UseTool;
    }
    private void UseTool(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;
    }

    public void MoveToDirection(Vector2 value)
    {
        CommandMoveToDirection(value);
    }
}