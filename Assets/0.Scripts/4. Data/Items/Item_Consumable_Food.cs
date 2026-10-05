using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable_Food", menuName = "Item/Consumable/Food")]
public class Item_Consumable_Food : Item_Consumable
{
    [Header("회복 수치")]
    public int hungerChange = 10;
    public int thirstyChange = -5;

    public virtual bool IsUsable(CharacterBase from, CharacterBase to)
    {
        if (to == null) return false;

        StatModule statModule = to.GetModule<StatModule>();
        if (statModule == null) return false;

        // 아무 효과도 줄 수 없는 상태라면 사용 불가
        if (hungerChange > 0 && statModule.Hunger.IsMax &&
            thirstyChange >= 0 && statModule.Thirst.IsMax)
        {
            return false;
        }

        return true;
    }

    public override bool OnUse(CharacterBase from, CharacterBase to)
    {
        if (!IsUsable(from, to))
            return false;

        StatModule statModule = to.GetModule<StatModule>();
        if (statModule == null)
            return false;

        bool used = false;

        // 배고픔 증가
        if (hungerChange > 0)
        {
            used |= statModule.IncreaseHunger(hungerChange);
        }
        else if (hungerChange < 0)
        {
            statModule.Hunger.Decrease(-hungerChange);
            used = true;
        }

        // 갈증 변화
        if (thirstyChange > 0)
        {
            used |= statModule.IncreaseThirst(thirstyChange);
        }
        else if (thirstyChange < 0)
        {
            statModule.Thirst.Decrease(-thirstyChange);
            used = true;
        }

        return used;
    }

    public virtual bool IsUsable(CharacterBase from, Vector3 position)
    {
        return false;
    }

    public override bool OnUse(CharacterBase from, Vector3 position)
    {
        return false;
    }
}