using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable_Food", menuName = "Item/Consumable/Food")]
public class Item_Consumable_Food : Item_Consumable
{
    public int hungerChange = 10;
    public int ThirstyChange = -5;

    public virtual bool IsUsable(CharacterBase from, CharacterBase to) => true;

    /*public override bool OnUse(CharacterBase from, CharacterBase to)
    {
        if (to == null) return false;

        StatModule statModule = to.GetModule<StatModule>();
        if (statModule == null) return false;

        statModule.Hunger.IncreaseCurrent(hungerChange);
        statModule.Thirst.IncreaseCurrent(ThirstyChange);

        return true;
    }*/

    public virtual bool IsUsable(CharacterBase from, Vector3 position) => true;

    public override bool OnUse(CharacterBase from, Vector3 position)
    {
        return false;
    }
}