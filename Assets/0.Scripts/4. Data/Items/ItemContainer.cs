using UnityEngine;

public enum ItemType
{
    None,
    Equipment = 99,
    Tool = 95,
    Herb = 90,
    Hallucinogen = 80,
    Mineral = 70,
    Gem = 60,
    Food = 50,
    Book = 40,
    Record = 30,
    Structure = 20,
    Misc = 10,
    Length
}

public enum ItemUseType
{
    None,
    Instant,
    Target,
    Placement
}

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Item/ItemBase")]
public class ItemContainer : InfoContainer
{
    [Header("아이템 정보")]
    public int id;
    [Space]
    [Header("아이템 세부사항")]
    public ItemType type;
    public ItemUseType useType;
    public int maxStack;

    public virtual int CompareByType(ItemContainer other)
    {
        if (other == null) return 1;
        int result = type - other.type;
        if (result != 0) return result;
        return id - other.id;
    }

    public virtual int CompareByType(ItemContainer mySlot, ItemSlot otherSlot)
    {
        return id - mySlot.id;
    }
}