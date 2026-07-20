using UnityEngine;
//using static Unity.Collections.Unicode;
//using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class AnimationModule : CharacterModule
{
    [SerializeField] Animator anim;
    [SerializeField] bool isRotationByMovement;

    public sealed override System.Type RegistrationType => typeof(AnimationModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        newOwner.OnLookAt -= AnimationByLookRotation;
        newOwner.OnLookAt += AnimationByLookRotation;
        newOwner.OnMovement -= AnimationByMovement;
        newOwner.OnMovement += AnimationByMovement;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        oldOwner.OnLookAt -= AnimationByLookRotation;
        oldOwner.OnMovement -= AnimationByMovement;
    }

    public void AnimationByLookRotation(Vector3 lookRotation)
    {
        if (!anim) return;
        anim.SetFloat("MoveX", lookRotation.x);
        anim.SetFloat("MoveY", lookRotation.y);
    }

    public void AnimationByMovement(Vector3 moveDelta)
    {
        if (!anim) return;
        if (isRotationByMovement && moveDelta.sqrMagnitude > 0)
        {
            AnimationByLookRotation(moveDelta.normalized);
        }
        anim.SetFloat("MoveSpeed", moveDelta.magnitude / Time.fixedDeltaTime);
    }

    //곡괭이
    public void PlayPickaxe()
    {
        if (!anim) return;
        anim.SetTrigger("PickAxeOn");
    }

    //호미
    public void PlayHoe()
    {
        if (!anim) return;
        anim.SetTrigger("HoeOn");
    }

    //잠자리채
    public void PlayBugnet()
    {
        if (!anim) return;
        anim.SetTrigger("BugnetOn");
    }

    //도끼
    public void PlayAxe()
    {
        if (!anim) return;
        anim.SetTrigger("AxeOn");
    }

    //낫
    public void PlaySickle()
    {
        if (!anim) return;
        anim.SetTrigger("SickleOn");
    }

    //삽
    public void PlayShovel()
    {
        if (!anim) return;
        anim.SetTrigger("ShovelOn");
    }

    //물뿌리개
    public void PlayWatering()
    {
        if (!anim) return;
        anim.SetTrigger("WateringOn");
    }

    //칼
    public void PlaySword()
    {
        if (!anim) return;
        anim.SetTrigger("SwordOn");
    }

    //활
    public void PlayArcher()
    {
        if (!anim) return;
        anim.SetTrigger("ArcherOn");
    }

    //데미지
    public void PlayDamage()
    {
        if (!anim) return;
        anim.SetTrigger("DamageOn");
    }

    //죽음
    public void PlayDeath()
    {
        if (!anim) return;
        anim.SetTrigger("DeathOn");
    }
    //낚시와 잠
}