using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ControllerBase : MonoBehaviour, IFunctionable
{
    CharacterBase _character;
    public CharacterBase Character => _character;

    //Controller와 Character를 연결하는 함수
    public virtual void RegistrationFunctions()
    {
        Possess(GetComponent<CharacterBase>());
    }

    //Controller와 Character의 연결을 끊는 함수
    public virtual void UnregistrationFunctions()
    {
        Unpossess();
    }

    //Character를 조종하기 시작할 때 실행되는 확장용 함수
    protected virtual void OnPossess(CharacterBase newCharacter) { }

    //특정 Character를 조종하도록 연결하는 함수
    public void Possess(CharacterBase target)
    {
        if (!target) return; 
        ControllerBase result = target.Possessed(this);
        if (result == this)
        {
            _character = target;
            OnPossess(target);
        }
    }

    //Controller가 Character의 조종을 해제했을 때 추가로 실행할 수 있는 메서드
    protected virtual void OnUnpossess(CharacterBase oldCharacter) { }

    //현재 조종하고 있는 Character와의 조종 관계를 해제하는 메서드
    public void Unpossess()
    {
        if (Character)
        {
            if (Character.Unpossessed(this))
            {
                OnUnpossess(Character);
            }
        }
        _character = null;
    }

    //Character의 MovementModule에 특정 방향으로 이동하라는 명령을 전달하는 메서드
    public void CommandMoveToDirection(Vector3 direction)
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.MoveToDirection(direction);
    }

    //Character에게 정지 명령을 전달하는 메서드
    public void CommandStop()
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.StopMovement();
    }
}