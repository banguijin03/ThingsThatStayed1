using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable_Food", menuName = "Item/Consumable/Food")]
public class Item_Consumable_Food : Item_Consumable
{
    public float hugerChange = 10.0f;
    public float ThirstyChange = -5.0f;

    public virtual bool IsUsable(CharacterBase from, CharacterBase to) => true;
    public override void OnUse(CharacterBase from, CharacterBase to)
    {

    }
    public virtual bool IsUsable(CharacterBase from, Vector3 position) => true;
    public override void OnUse(CharacterBase from, Vector3 position)
    {

    }
}
