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

    [SerializeField] float rollSpeed = 10f;
    [SerializeField] float rollDuration = 0.3f;
    [SerializeField] float rollCooldown = 1f;

    bool isRolling = false;
    Vector3 rollDirection;
    float rollTimer;
    float rollCooldownTimer;

    Animator animator;

    // Roll 충돌 검사
    Collider2D characterCollider;
    ContactFilter2D rollContactFilter;
    RaycastHit2D[] rollHits = new RaycastHit2D[4];


    // =========================
    // Registration
    // =========================

    // 이동 및 Shift, Roll 입력 이벤트를 등록
    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        animator = GetComponentInChildren<Animator>();

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


    // 등록했던 이벤트를 해제
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

    // 실제 이동을 처리하고 이동 결과를 캐릭터에게 알림
    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position;

        PhysicsUpdate(deltaTime);

        Vector3 positionDelta = transform.position - originPosition;

        Owner.MovementNotify(positionDelta);
    }


    // 방향 이동과 목적지 이동 중 알맞은 이동 방식을 실행
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


    // =========================
    // Normal Movement
    // =========================

    // 현재 이동 상태에 따른 이동 속도를 반환
    public virtual float GetMoveSpeed()
    {
        return isShift ? 7.0f : 5.0f;
    }


    // 이동 속도에 deltaTime을 적용해 실제 이동량을 계산
    public virtual float GetMoveSpeed(float deltaTime)
        => GetMoveSpeed() * deltaTime;


    // 캐릭터의 위치를 실제로 이동
    public virtual void Translate(Vector3 delta)
    {
        transform.position += delta;
    }


    // 지정된 방향으로 캐릭터를 이동
    public virtual void UpdateToDirection(float deltaTime)
    {
        float currentMoveSpeed = GetMoveSpeed(deltaTime);

        Translate(currentMoveSpeed * targetDirection.Value);
    }


    // 지정된 목적지를 향해 캐릭터를 이동
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


    // 이동할 목적지와 허용 거리를 설정
    public virtual void MoveToDestination(
        Vector3 destination,
        float tolerance)
    {
        targetDirection = null;
        targetDestination = destination;
        targetTolerance = tolerance;
    }


    // 이동방향 설정 및 해당 방향 바라봄
    public virtual void MoveToDirection(Vector3 direction)
    {
        targetDestination = null;
        targetDirection = direction.normalized;

        if (targetDirection.Value != Vector3.zero)
        {
            Owner.LookAtNotify(targetDirection.Value);
        }
    }


    // Shift로 달리기 상태인지 확인
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


        // 현재 이동 방향 저장
        rollDirection = targetDirection.Value.normalized;

        isRolling = true;

        rollTimer = rollDuration;

        rollCooldownTimer = rollCooldown;


        // 기존 이동 중단
        StopMovement();

        animator?.SetTrigger("RollOn");
    }


    void RollUpdate(float deltaTime)
    {
        float moveDistance = rollSpeed * deltaTime;


        // =========================
        // Roll 충돌 검사
        // =========================

        if (characterCollider != null)
        {
            int hitCount = characterCollider.Cast(
                rollDirection,
                rollContactFilter,
                rollHits,
                moveDistance
            );


            float closestDistance = moveDistance;

            bool blocked = false;


            for (int i = 0; i < hitCount; i++)
            {
                if (rollHits[i].collider == null)
                    continue;


                if (rollHits[i].distance < closestDistance)
                {
                    closestDistance = rollHits[i].distance;

                    blocked = true;
                }
            }


            // 앞에 벽이나 맵 경계가 있는 경우
            if (blocked)
            {
                // 벽에 딱 붙지 않도록 약간의 여유를 둠
                float safeDistance =
                    Mathf.Max(0f, closestDistance - 0.02f);


                if (safeDistance > 0f)
                {
                    Translate(
                        rollDirection * safeDistance
                    );
                }


                // 벽에 닿으면 구르기 종료
                rollTimer = 0f;

                isRolling = false;

                return;
            }
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

    // 이동 중단
    public virtual void StopMovement()
    {
        targetDestination = null;
        targetDirection = null;
    }
}