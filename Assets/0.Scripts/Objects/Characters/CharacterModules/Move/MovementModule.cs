using UnityEngine;

public class MovementModule : CharacterModule, IRunnable
{
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;

    public sealed override System.Type RegistrationType => typeof(MovementModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnPhysicsCharacter -= MovementUpdate;
    }
    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position;                
        PhysicsUpdate(deltaTime);                                   
        Vector3 positionDelta = transform.position - originPosition;
        Owner.MovementNotify(positionDelta);                            
    }
    public void PhysicsUpdate(float deltaTime)
    {
        if (targetDirection is not null) UpdateToDirection(deltaTime);
        else if (targetDestination is not null) UpdateToDestination(deltaTime);
    }
    public virtual float GetMoveSpeed() => 5.0f;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;
    public virtual void Translate(Vector3 delta)
    {
        transform.position += delta;
    }
    public virtual void UpdateToDirection(float deltaTime)
    {
        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * targetDirection.Value);
    }
    public virtual void UpdateToDestination(float deltaTime)
    {
        Vector3 currentMoveDirection = (targetDestination.Value - transform.position);
        float distance = currentMoveDirection.magnitude;
        if (distance > targetTolerance)
        {
            currentMoveDirection.Normalize();

            float currentMoveSpeed = GetMoveSpeed(deltaTime);
            
            float resultMoveSpeed = Mathf.Min(currentMoveSpeed, distance);

            Translate(resultMoveSpeed * currentMoveDirection);
        }
    }

    public virtual void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDirection = null; 
        targetDestination = destination;
        targetTolerance = tolerance;
    }
    public virtual void MoveToDirection(Vector3 direction)
    {
        targetDestination = null;
        targetDirection = direction.normalized;

        if (targetDirection.Value != Vector3.zero)
        {
            Owner.LookAtNotify(targetDirection.Value);
        }
    }
    public virtual void StopMovement()
    {
        targetDestination = null; 
        targetDirection = null; 
    }

}