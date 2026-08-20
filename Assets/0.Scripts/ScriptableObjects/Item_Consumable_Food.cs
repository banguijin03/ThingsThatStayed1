using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable_Food", menuName = "Item/Consumable/Food")]
public class Item_Consumable_Food : Item_Consumable
{
    public int hungerChange = 10;
    public int ThirstyChange = -5;

    public virtual bool IsUsable(CharacterBase from, CharacterBase to) => true;
    public override void OnUse(CharacterBase from, CharacterBase to)
    {
        StatModule statModule = to.GetModule<StatModule>();

        if (statModule == null) return;

        statModule.Hunger.IncreaseCurrent(hungerChange);
        statModule.Thirst.IncreaseCurrent(ThirstyChange);
    }

    public virtual bool IsUsable(CharacterBase from, Vector3 position) => true;
    public override void OnUse(CharacterBase from, Vector3 position)
    {

    }
}
