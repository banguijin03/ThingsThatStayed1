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

        Debug.Log($"[CursorSlot] 클릭됨 / Hover: {InputManager.CursorHoverObject}");

        GameObject currentHover = InputManager.CursorHoverObject;
        if (!currentHover)
        {
            Debug.Log("[CursorSlot] Hover 없음");
            return;
        }

        UI_ItemSlotInfo currentSlotInfo = currentHover.GetComponentInParent<UI_ItemSlotInfo>();

        Debug.Log($"[CursorSlot] SlotInfo: {currentSlotInfo}");

        if (currentSlotInfo == null)
        {
            Debug.Log("[CursorSlot] UI_ItemSlotInfo를 찾지 못함");
            return;
        }

        ItemSlot targetSlot = currentSlotInfo.ConnectedSlot;

        Debug.Log($"[CursorSlot] TargetSlot: {targetSlot}");

        if (targetSlot == null) return;
        if (ConnectedSlot == null) return;

        Debug.Log("[CursorSlot] LeftClick 실행");

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