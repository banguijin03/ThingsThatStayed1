using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public enum ItemType
{
    None,
    Equipment       = 99,    // 의복
    Tool            = 95,    // 도구
    Herb            = 90,    // 약초
    Hallucinogen    = 80,    // 환각제
    Mineral         = 70,    // 광물
    Gem             = 60,    // 보석
    Food            = 50,    // 요리
    Book            = 40,    // 책
    Record          = 30,    // 기록
    Structure       = 20,    // 시설물
    Misc            = 10,    // 기타
    Length
        //등급: 가죽/나무, 구리, 철, 은, 금, 백금, 미스릴, 오리하르콘, 아다만티움, 고대
}

//public enum ItemType
//{
//    Equipment, Consumable, Material, Miscellaneous, Quest, Impotrtant, Resource,
//    Length
//}

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Item/ItemBase")]
public class ItemContainer : InfoContainer
{
    [Header("아이템 정보")]
    public int id;
    [Space]
    [Header("아이템 세부사항")]
    public ItemType type;
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
