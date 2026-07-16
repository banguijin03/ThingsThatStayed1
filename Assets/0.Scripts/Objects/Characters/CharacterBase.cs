using System.Collections.Generic;
using UnityEngine;

public delegate void MovementEvent(Vector3 move);
public delegate void LookAtEvent(Vector3 direction);
public delegate void DamageEvent(GameObject damageCauser, ControllerBase instigator, float damage);

public class CharacterBase : MonoBehaviour
{
    public event MovementEvent OnMovement;
    public void MovementNotify(Vector3 move) => OnMovement?.Invoke(move);

    public event LookAtEvent OnLookAt;
    public void LookAtNotify(Vector3 direction) => OnLookAt?.Invoke(direction);

    public event DamageEvent OnDamage;
    public void DamageNotify(GameObject damageCauser, ControllerBase instigator, float damage)
        => OnDamage?.Invoke(damageCauser, instigator, damage);

    ControllerBase _controller;
    public ControllerBase Controller => _controller;

    protected Vector3 _lookRotation;
    public Vector3 LookRotation => _lookRotation;

    public virtual string DisplayName => "character";

    Dictionary<System.Type, List<CharacterModule>> moduleDictionary = new();

    public void AddModule(System.Type wantType, CharacterModule wantModule)
    {
        if (!moduleDictionary.TryGetValue(wantType, out List<CharacterModule> list))
        {
            list = new List<CharacterModule>();
            moduleDictionary.Add(wantType, list);
        }

        list.Add(wantModule);
        wantModule.OnRegistration(this);
    }

    public void AddAllModuleFromObject(GameObject target)
    {
        if (!target) return;

        foreach (CharacterModule currentModule in target.GetComponentsInChildren<CharacterModule>())
        {
            AddModule(currentModule.RegistrationType, currentModule);
        }
    }

    public void RemoveModule(System.Type wantType)
    {
        if (!moduleDictionary.TryGetValue(wantType, out List<CharacterModule> list))
            return;

        foreach (CharacterModule module in list)
        {
            module.OnUnregistration(this);
        }

        moduleDictionary.Remove(wantType);
    }

    public void RemoveAllModule()
    {
        foreach (List<CharacterModule> list in moduleDictionary.Values)
        {
            foreach (CharacterModule module in list)
            {
                module.OnUnregistration(this);
            }
        }

        moduleDictionary.Clear();
    }

    public T GetModule<T>() where T : CharacterModule
    {
        if (moduleDictionary.TryGetValue(typeof(T), out List<CharacterModule> list))
        {
            if (list.Count > 0)
                return list[0] as T;
        }

        return null;
    }

    public List<T> GetModules<T>() where T : CharacterModule
    {
        List<T> result = new();

        if (moduleDictionary.TryGetValue(typeof(T), out List<CharacterModule> list))
        {
            foreach (CharacterModule module in list)
            {
                result.Add(module as T);
            }
        }

        return result;
    }

    protected virtual void OnPossessed(ControllerBase newController) { }

    public ControllerBase Possessed(ControllerBase from)
    {
        if (Controller) Unpossessed();

        _controller = from;
        AddAllModuleFromObject(gameObject);
        OnPossessed(Controller);

        return Controller;
    }

    protected virtual void OnUnpossessed(ControllerBase oldController) { }

    public void Unpossessed()
    {
        if (Controller)
            OnUnpossessed(Controller);

        RemoveAllModule();
        _controller = null;
    }

    public bool Unpossessed(ControllerBase oldController)
    {
        if (Controller != oldController)
            return false;

        Unpossessed();
        return true;
    }
}