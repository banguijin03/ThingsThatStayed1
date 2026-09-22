using UnityEngine;

public class UI_ItemCursorSlotInfo : UI_ItemSlotInfo
{
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        ConnectSlot(Inventory.cursorSlot);

        InputManager.OnMouseMove -= MoveToMouse;
        InputManager.OnMouseMove += MoveToMouse;

        InputManager.OnMouseLeftButton -= LeftButton;
        InputManager.OnMouseLeftButton += LeftButton;

        InputManager.OnMouseRightButton -= RightButton;
        InputManager.OnMouseRightButton += RightButton;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);

        DisconnectSlot();

        InputManager.OnMouseMove -= MoveToMouse;
        InputManager.OnMouseLeftButton -= LeftButton;
        InputManager.OnMouseRightButton -= RightButton;
    }

    void LeftButton(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;

        GameObject currentHover = InputManager.CursorHoverObject;
        if (!currentHover) return;

        UI_ItemSlotInfo currentSlotInfo = currentHover.GetComponentInParent<UI_ItemSlotInfo>();


        if (currentSlotInfo == null)  return;

        ItemSlot targetSlot = currentSlotInfo.ConnectedSlot;


        if (targetSlot == null) return;
        if (ConnectedSlot == null) return;

        ConnectedSlot.LeftClick(targetSlot);
    }

    void RightButton(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;

        GameObject currentHover = InputManager.CursorHoverObject;
        if (!currentHover) return;

        UI_ItemSlotInfo currentSlotInfo = currentHover.GetComponentInParent<UI_ItemSlotInfo>();
        if (currentSlotInfo == null) return;

        ItemSlot targetSlot = currentSlotInfo.ConnectedSlot;
        if (targetSlot == null) return;

        if (ConnectedSlot == null) return;

        ConnectedSlot.RightClick(targetSlot);
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition;
    }
}