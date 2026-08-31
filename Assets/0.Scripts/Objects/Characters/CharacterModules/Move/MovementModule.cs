using UnityEngine;

public class MovementModule : CharacterModule, IRunnable
{
    bool isShift = false;
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;
    public Vector3 CurrentDirection => targetDirection ?? Vector3.zero;

    public sealed override System.Type RegistrationType => typeof(MovementModule);

    //이동 및 Shift 입력 이벤트를 등록
    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;

        InputManager.OnShift -= ShiftMove;
        InputManager.OnShift += ShiftMove;
    }

    //등록했던 이벤트를 해제
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnPhysicsCharacter -= MovementUpdate;
        InputManager.OnShift -= ShiftMove;
    }

    //실제 이동을 처리하고 이동 결과를 캐릭터에게 알림
    public void MovementUpdate(float deltaTime)
    {
        //기존좌표
        Vector3 originPosition = transform.position;
        PhysicsUpdate(deltaTime);

        //현재 좌표
        Vector3 positionDelta = transform.position - originPosition;
        Owner.MovementNotify(positionDelta);
    }

    //방향 이동과 목적지 이동 중 알맞은 이동 방식을 실행
    public void PhysicsUpdate(float deltaTime)
    {
        if (targetDirection is not null) UpdateToDirection(deltaTime);
        else if (targetDestination is not null) UpdateToDestination(deltaTime);
    }

    //현재 이동 상태에 따른 이동 속도를 반환
    public virtual float GetMoveSpeed()
    {
        return isShift ? 7.0f : 5.0f;
    }

    //이동 속도에 deltaTime을 적용해 실제 이동량을 계산
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;

    //캐릭터의 위치를 실제로 이동
    public virtual void Translate(Vector3 delta)
    {
        transform.position += delta;
    }

    //지정된 방향으로 캐릭터를 이동
    public virtual void UpdateToDirection(float deltaTime)
    {
        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * targetDirection.Value);
    }

    //지정된 목적지를 향해 캐릭터를 이동
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

    //이동할 목적지와 허용 거리를 설정
    public virtual void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDirection = null; 
        targetDestination = destination;
        targetTolerance = tolerance;
    }

    //이동방향 설정 및 해당 방향 바라봄
    public virtual void MoveToDirection(Vector3 direction)
    {
        targetDestination = null;
        targetDirection = direction.normalized;

        if (targetDirection.Value != Vector3.zero)
        {
            Owner.LookAtNotify(targetDirection.Value);
        }
    }

    //shift로 달리기 상태인지 확인
    public void ShiftMove(bool value)
    {
        isShift = value;
    }

    //이동 중단
    public virtual void StopMovement()
    {
        targetDestination = null; 
        targetDirection = null; 
    }

}