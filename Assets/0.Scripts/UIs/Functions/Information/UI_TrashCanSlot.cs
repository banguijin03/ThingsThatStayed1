using UnityEngine;

public class UI_TrashCanSlot : UI_ItemSlotInfo
{
    protected override void VisualUpdate(ItemSlot targetSlot)
    {
        iconImage.enabled = true;
    }
}