using UnityEngine;

public class MovementModule : CharacterModule, IRunnable
{
    bool isShift = false;

    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;

    public Vector3 CurrentDirection => targetDirection ?? Vector3.zero;

    public sealed override System.Type RegistrationType => typeof(MovementModule);


    // =========================
    // Roll 설정
    // =========================

    [SerializeField] float rollSpeed = 15f;
    [SerializeField] float rollDuration = 0.3f;
    [SerializeField] float rollCooldown = 3f;

    // 벽에 너무 딱 붙지 않도록 하는 여유 거리
    [SerializeField] float rollSkin = 0.02f;

    bool isRolling = false;
    Vector3 rollDirection;
    float rollTimer;
    float rollCooldownTimer;

    Animator animator;


    // =========================
    // Roll 충돌 검사
    // =========================

    Collider2D characterCollider;

    ContactFilter2D rollContactFilter;

    // 매번 새로 만들지 않고 재사용
    RaycastHit2D[] rollHits = new RaycastHit2D[4];


    //이동 및 Shift, Roll 입력 이벤트를 등록
    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        animator = GetComponentInChildren<Animator>();


        // 플레이어 Collider 가져오기
        characterCollider = GetComponent<Collider2D>();


        // Roll 충돌 검사 설정
        rollContactFilter = new ContactFilter2D();

        rollContactFilter.SetLayerMask(
            Physics2D.GetLayerCollisionMask(gameObject.layer)
        );

        rollContactFilter.useTriggers = false;


        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;

        InputManager.OnShift -= ShiftMove;
        InputManager.OnShift += ShiftMove;

        InputManager.OnRoll -= RollInput;
        InputManager.OnRoll += RollInput;
    }


    //등록했던 이벤트를 해제
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);

        GameManager.OnPhysicsCharacter -= MovementUpdate;

        InputManager.OnShift -= ShiftMove;
        InputManager.OnRoll -= RollInput;
    }


    // =========================
    // Movement
    // =========================

    //실제 이동을 처리하고 이동 결과를 캐릭터에게 알림
    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position;

        PhysicsUpdate(deltaTime);

        Vector3 positionDelta = transform.position - originPosition;

        Owner.MovementNotify(positionDelta);
    }


    //방향 이동과 목적지 이동 중 알맞은 이동 방식을 실행
    public void PhysicsUpdate(float deltaTime)
    {
        // Roll 중이면 일반 이동을 하지 않음
        if (isRolling)
        {
            RollUpdate(deltaTime);
            return;
        }


        if (rollCooldownTimer > 0f)
            rollCooldownTimer -= deltaTime;


        if (targetDirection is not null)
            UpdateToDirection(deltaTime);
        else if (targetDestination is not null)
            UpdateToDestination(deltaTime);
    }


    //현재 이동 상태에 따른 이동 속도를 반환
    public virtual float GetMoveSpeed()
    {
        return isShift ? 7.0f : 5.0f;
    }


    //이동 속도에 deltaTime을 적용해 실제 이동량을 계산
    public virtual float GetMoveSpeed(float deltaTime)
        => GetMoveSpeed() * deltaTime;


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
        Vector3 currentMoveDirection =
            (targetDestination.Value - transform.position);

        float distance = currentMoveDirection.magnitude;


        if (distance > targetTolerance)
        {
            currentMoveDirection.Normalize();

            float currentMoveSpeed = GetMoveSpeed(deltaTime);

            float resultMoveSpeed =
                Mathf.Min(currentMoveSpeed, distance);

            Translate(resultMoveSpeed * currentMoveDirection);
        }
    }


    //이동할 목적지와 허용 거리를 설정
    public virtual void MoveToDestination(
        Vector3 destination,
        float tolerance)
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


    // =========================
    // Roll
    // =========================

    void RollInput(bool value)
    {
        if (!value)
            return;

        if (isRolling)
            return;

        if (rollCooldownTimer > 0f)
            return;

        if (targetDirection is null)
            return;

        if (targetDirection.Value == Vector3.zero)
            return;


        rollDirection = targetDirection.Value.normalized;

        isRolling = true;

        rollTimer = rollDuration;

        rollCooldownTimer = rollCooldown;


        animator?.SetTrigger("RollOn");
    }


    // =========================
    // Roll 이동 + 충돌 검사
    // =========================

    void RollUpdate(float deltaTime)
    {
        float moveDistance = rollSpeed * deltaTime;


        // 플레이어 Collider가 없으면
        // 기존 Roll 방식으로 이동
        if (characterCollider == null)
        {
            Translate(rollDirection * moveDistance);

            rollTimer -= deltaTime;

            if (rollTimer <= 0f)
            {
                rollTimer = 0f;
                isRolling = false;
            }

            return;
        }


        // =========================
        // 앞으로 충돌하는지 검사
        // =========================

        int hitCount = characterCollider.Cast(
            rollDirection,
            rollContactFilter,
            rollHits,
            moveDistance
        );


        // 이동 가능한 최대 거리
        float allowedDistance = moveDistance;


        for (int i = 0; i < hitCount; i++)
        {
            if (rollHits[i].collider == null)
                continue;


            if (rollHits[i].distance < allowedDistance)
            {
                allowedDistance = rollHits[i].distance;
            }
        }


        // =========================
        // 벽에 닿는 경우
        // =========================

        if (allowedDistance < moveDistance)
        {
            // 벽에 너무 딱 붙지 않도록 약간 빼줌
            allowedDistance = Mathf.Max(
                0f,
                allowedDistance - rollSkin
            );


            if (allowedDistance > 0f)
            {
                Translate(
                    rollDirection * allowedDistance
                );
            }


            // 벽에 닿았으므로 Roll 종료
            rollTimer = 0f;

            isRolling = false;

            return;
        }


        // =========================
        // 정상 Roll 이동
        // =========================

        Translate(
            rollDirection * moveDistance
        );


        rollTimer -= deltaTime;


        if (rollTimer <= 0f)
        {
            rollTimer = 0f;

            isRolling = false;
        }
    }


    // =========================
    // Movement Stop
    // =========================

    //이동 중단
    public virtual void StopMovement()
    {
        targetDestination = null;
        targetDirection = null;
    }
}