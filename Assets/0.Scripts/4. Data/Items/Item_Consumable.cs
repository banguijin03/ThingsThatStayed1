using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable", menuName = "Item/Consumable")]
public class Item_Consumable : ItemContainer
{
    public virtual bool OnUse(CharacterBase from, CharacterBase to)
    {
        return false;
    }

    public virtual bool OnUse(CharacterBase from, Vector3 position)
    {
        return false;
    }
}