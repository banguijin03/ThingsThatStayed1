using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable", menuName = "Item/Consumable")]
public class Item_Consumable : ItemContainer
{
    public virtual void OnUse(CharacterBase from, CharacterBase to)
    {

    }

    public virtual void OnUse(CharacterBase from, Vector3 position)
    {
        
    }
}
