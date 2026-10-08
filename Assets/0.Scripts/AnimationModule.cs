using UnityEngine;

public class AnimationModule : CharacterModule
{
    [SerializeField] Animator anim;
    [SerializeField] bool isRotationByMovement;

    private StatModule statModule;

    public sealed override System.Type RegistrationType => typeof(AnimationModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        newOwner.OnLookAt -= AnimationByLookRotation;
        newOwner.OnLookAt += AnimationByLookRotation;

        newOwner.OnMovement -= AnimationByMovement;
        newOwner.OnMovement += AnimationByMovement;

        statModule = newOwner.GetModule<StatModule>();

        if (statModule != null)
        {
            statModule.OnDeath -= AnimationByDeath;
            statModule.OnDeath += AnimationByDeath;
        }
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        if (statModule != null)
        {
            statModule.OnDeath -= AnimationByDeath;
            statModule = null;
        }

        oldOwner.OnLookAt -= AnimationByLookRotation;
        oldOwner.OnMovement -= AnimationByMovement;

        base.OnUnregistration(oldOwner);
    }

    // 방향 처리
    public void AnimationByLookRotation(Vector3 lookRotation)
    {
        if (!anim) return;

        anim.SetFloat("MoveX", lookRotation.x);
        anim.SetFloat("MoveY", lookRotation.y);
    }

    // 이동 애니메이션
    public void AnimationByMovement(Vector3 moveDelta)
    {
        if (!anim) return;

        if (isRotationByMovement && moveDelta.sqrMagnitude > 0)
        {
            AnimationByLookRotation(moveDelta.normalized);
        }

        anim.SetFloat(
            "MoveSpeed",
            moveDelta.magnitude / Time.fixedDeltaTime
        );
    }

    // 사망 애니메이션
    private void AnimationByDeath()
    {
        if (!anim) return;

        anim.SetBool("Death", true);
        UIManager.ClaimOpenUI(UIType.juge);
    }
}