using System;
using UnityEngine;

public class UI_ScreenChanger : OpenableUIBase
{
    [SerializeField] Animator anim;
    Action AnimEndFunction;
    bool isChangingIn;

    public void ChangeStart(Action newFunction = null)
    {
        InputManager.SetInputLocked(true);
        isChangingIn = false;

        AnimEndFunction = newFunction;

        if (anim) anim.SetTrigger("Out");
        else OnAnimEnd();
    }

    public void ChangeEnd(Action newFunction = null)
    {
        isChangingIn = true;
        AnimEndFunction = newFunction;

        if (anim) anim.SetTrigger("In");
        else OnAnimEnd();
    }

    public void OnAnimEnd()
    {
        AnimEndFunction?.Invoke();

        if (isChangingIn)
            InputManager.SetInputLocked(false);
    }
}