using System;
using UnityEngine;

public delegate void ItemSlotChangeEvent(ItemSlot changedSlot);

public class ItemSlot
{
    //이 칸에 들어있는 아이템의 정보
    [SerializeField] ItemContainer item;
    //이 칸 만의 정보
    [SerializeField] int currentStack;


    public event ItemSlotChangeEvent OnItemSlotChanged;

    public void NoticeChanged() => OnItemSlotChanged?.Invoke(this);

    public virtual bool Containable(ItemContainer wantItem)
    {
        if (!wantItem)                  return false;
        if (item && item != wantItem)   return false;
        if (GetIsMax())                 return false;

        return true;
    }
    public ItemContainer GetItem()  => item;
    public int GetStackable(ItemContainer wantItem) => wantItem ? wantItem.maxStack - currentStack : 0;
    public int GetStackable() => GetStackable(item);
    public int GetStack()           => currentStack;
    public bool GetIsMax()          => item ? currentStack >= item.maxStack : false;
    public bool GetIsEmpty()        => !item || currentStack <= 0;

    public int Clear()
    {
        item = null;
        //  현재스택
        int removed = currentStack;
        currentStack = 0;
        return removed;
    }

    public int AddItem(ItemContainer wantItem, int amount)
    {
        if (amount <= 0) return 0;
        else if (!Containable(wantItem)) return amount;

        item = wantItem;
        int stackable = Mathf.Min(item.maxStack - currentStack, amount);
        currentStack += stackable;

        return amount - stackable;
    }

    public int RemoveItem(ItemContainer wantItem)
    {
        if (!wantItem) return 0;
        if (GetIsEmpty()) return 0;
        if(item != wantItem) return 0;

        return Clear();
    }
    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        if (!wantItem) return amount;
        if (GetIsEmpty()) return amount;
        if (item != wantItem) return amount;
        if (amount >= currentStack) return amount - Clear();
        currentStack -= amount;
        return 0;
    }

    public void ExchangeItem(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;
        ItemContainer wasItem = item;
        int wasStack = currentStack;
        //아이템을 가져옴
        item = wantSlot.item;
        currentStack = wantSlot.currentStack;
        //원래 가지고 있던걸로 갱신 
        wantSlot.item = wasItem;
        wantSlot.currentStack = wasStack;
    }

    public int GiveItem(ItemSlot wantSlot) => GiveItem(wantSlot, currentStack);
    public int GiveItem(ItemSlot wantSlot, int amount)
    {
        if (wantSlot is null) return amount;
        if (!item) return amount;
        if (currentStack <= 0 || amount <= 0) return amount;

        ItemContainer targetItem = item;
        amount = Mathf.Min(amount, wantSlot.GetStackable(targetItem));
        amount -= RemoveItem(targetItem, amount);
        amount = wantSlot.AddItem(targetItem, amount);
        return amount;
    }

    public void LeftClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        if (InputManager.IsShif)
        {
            if (wantSlot.GetIsEmpty())
            {
                if (GetIsEmpty()) return;
                else if (wantSlot.Containable(item))
                {
                    GiveItem(wantSlot, Mathf.CeilToInt(currentStack * 0.5f));
                }
            }
            else if (Containable(wantSlot.item))
            {
                wantSlot.GiveItem(this, Mathf.CeilToInt(wantSlot.currentStack * 0.5f));
            }
        }

        else
        {
            if (wantSlot.Containable(item))
            {
                GiveItem(wantSlot);
            }
            else
            {
                ExchangeItem(wantSlot);
            }
        }
        NoticeChanged();
        wantSlot.NoticeChanged();
    }
    public void RightClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

       

        if (GetIsEmpty())
        {
            if (wantSlot.GetIsEmpty()) return;
            if (Containable(wantSlot.item)) wantSlot.GiveItem(this, 1);
        }
        else
        {
            if (wantSlot.Containable(item)) GiveItem(wantSlot, 1);
            else return;
        }
    }
}