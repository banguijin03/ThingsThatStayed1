using NUnit.Framework.Constraints;
using UnityEngine;

public class ControllerBase : MonoBehaviour, IFunctionable
{
    CharacterBase _character;
    public CharacterBase Character => _character;

    public virtual void RegistrationFunctions()
    {
        Possess(GetComponent<CharacterBase>());
    }
    public virtual void UnregistrationFunctions()
    {
        Unpossess();
    }

    protected virtual void OnPossess(CharacterBase newCharacter) { }
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

    protected virtual void OnUnpossess(CharacterBase oldCharacter) { }
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

    public void CommandMoveToDirection(Vector3 direction)
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.MoveToDirection(direction);
    }

    public void CommandStop()
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.StopMovement();
    }
}