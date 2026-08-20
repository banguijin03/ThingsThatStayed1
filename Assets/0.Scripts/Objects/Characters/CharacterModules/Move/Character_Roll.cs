using System;
using UnityEngine;

public class Character_Roll : CharacterModule
{
    Animator animator;
    MovementModule movement;

    Vector3 currentInputDirection;
    Vector3 rollDirection;

    float rollTimer;
    float rollCooldownTimer;

    [SerializeField] float rollSpeed = 10f;
    [SerializeField] float rollDuration = 0.3f;
    [SerializeField] float rollCooldown = 1f;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        animator = GetComponentInChildren<Animator>();
        movement = Owner.GetModule<MovementModule>();

        InputManager.OnRoll -= RollInput;
        InputManager.OnRoll += RollInput;

        InputManager.OnMove -= MoveInput;
        InputManager.OnMove += MoveInput;

        GameManager.OnPhysicsCharacter -= RollUpdate;
        GameManager.OnPhysicsCharacter += RollUpdate;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        InputManager.OnRoll -= RollInput;
        InputManager.OnMove -= MoveInput;
        GameManager.OnPhysicsCharacter -= RollUpdate;

        base.OnUnregistration(oldOwner);
    }

    void MoveInput(Vector2 value)
    {
        currentInputDirection = new Vector3(value.x, value.y, 0f);
    }

    void RollInput(bool value)
    {
        if (!value)
            return;

        if (rollTimer > 0f)
            return;

        if (rollCooldownTimer > 0f)
            return;

        if (currentInputDirection == Vector3.zero)
            return;

        rollDirection = currentInputDirection.normalized;
        rollTimer = rollDuration;
        rollCooldownTimer = rollCooldown;

        movement.StopMovement();

        animator.SetTrigger("RollOn");
    }

    void RollUpdate(float deltaTime)
    {
        if (rollCooldownTimer > 0f)
            rollCooldownTimer -= deltaTime;

        if (rollTimer <= 0f)
            return;

        transform.position += rollDirection * rollSpeed * deltaTime;

        rollTimer -= deltaTime;

        if (rollTimer <= 0f)
        {
            rollTimer = 0f;

            if (currentInputDirection != Vector3.zero)
            {
                movement.MoveToDirection(currentInputDirection);
            }
        }
    }
}
