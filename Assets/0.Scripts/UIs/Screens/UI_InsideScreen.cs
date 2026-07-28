using UnityEngine;

public class UI_InsideScreen : UI_ScreenBase
{
    [SerializeField] QuickSlot quickSlot;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        quickSlot.Registration(manager);
    }

    public override void Unregistration(UIManager manager)
    {
        quickSlot.Unregistration(manager);

        base.Unregistration(manager);
    }
}