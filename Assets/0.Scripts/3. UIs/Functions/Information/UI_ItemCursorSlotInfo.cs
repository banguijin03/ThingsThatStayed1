using System;
using UnityEngine;

public class UI_ItemCursorSlotInfo : UI_ItemSlotInfo
{
    // 일반 슬롯과 다른점
    // 1. 마우스를 따라다닌다.
    // 2. 등록되는 시점이 그냥 시작했을 때

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

        // 퀵슬롯을 클릭한 경우 커서 슬롯은 무시
        if (currentHover.GetComponentInParent<UI_QuickSlotBackground>() != null) return;

        UI_ItemSlotInfo currentSlotInfo = currentHover.GetComponentInParent<UI_ItemSlotInfo>();
        if (currentSlotInfo == null) return;

        ConnectedSlot?.LeftClick(currentSlotInfo.ConnectedSlot);
    }

    void RightButton(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;

        GameObject currentHover = InputManager.CursorHoverObject;
        if (!currentHover) return;

        // 퀵슬롯을 클릭한 경우 커서 슬롯은 무시
        if (currentHover.GetComponentInParent<UI_QuickSlotBackground>() != null) return;

        UI_ItemSlotInfo currentSlotInfo = currentHover.GetComponentInParent<UI_ItemSlotInfo>();
        if (currentSlotInfo == null) return;

        ConnectedSlot?.RightClick(currentSlotInfo.ConnectedSlot);
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition;
    }
}