using System.Collections;
using UnityEngine;

public class ItemUseManager : ManagerBase
{
    UI_QuickSlotBackground quickSlot;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        quickSlot = FindAnyObjectByType<UI_QuickSlotBackground>(FindObjectsInactive.Include);

        InputManager.OnMouseLeftButton -= UseCurrentItem;
        InputManager.OnMouseLeftButton += UseCurrentItem;

        yield return null;
    }

    protected override void OnDisconnected()
    {
        InputManager.OnMouseLeftButton -= UseCurrentItem;
        quickSlot = null;
    }

    void UseCurrentItem(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;
        if (quickSlot == null) return;

        ItemSlot currentSlot = quickSlot.CurrentSlot;
        if (currentSlot == null || currentSlot.GetIsEmpty()) return;

        ItemContainer item = currentSlot.GetItem();
        if (item == null) return;

        GameObject target = InputManager.CursorHoverObject;

        if (item is Item_Consumable consumable)
        {
            UseConsumable(consumable, currentSlot, target, worldPosition);
            return;
        }

        if (item.type == ItemType.Structure)
        {
            UseStructure(item, currentSlot, worldPosition);
            return;
        }

        if (item.type == ItemType.Tool)
        {
            UseTool(item, currentSlot, target);
            return;
        }
    }

    void UseConsumable(Item_Consumable item, ItemSlot slot, GameObject target, Vector3 worldPosition)
    {
    }

    void UseStructure(ItemContainer item, ItemSlot slot, Vector3 worldPosition)
    {
    }

    void UseTool(ItemContainer item, ItemSlot slot, GameObject target)
    {
    }
}