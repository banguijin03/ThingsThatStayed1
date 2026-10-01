using UnityEngine;

public class CharacterModule : MonoBehaviour, IFunctionable
{
    public virtual System.Type RegistrationType => typeof(CharacterModule);

    CharacterBase _owner;
    public CharacterBase Owner => _owner;

    public virtual void RegistrationFunctions()
    {
        CharacterBase owner = GetComponent<CharacterBase>();
        if (owner == null) return;

        OnRegistration(owner);
    }

    public virtual void UnregistrationFunctions()
    {
        OnUnregistration(_owner);
    }

    public virtual void OnRegistration(CharacterBase newOwner)
    {
        _owner = newOwner;
    }

    public virtual void OnUnregistration(CharacterBase oldOwner)
    {
        _owner = null;
    }
}