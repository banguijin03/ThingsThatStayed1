using UnityEngine;

public class TrashCanSlot : ItemSlot
{
    public override bool Containable(ItemContainer wantItem)
    {
        return true;
    }

}