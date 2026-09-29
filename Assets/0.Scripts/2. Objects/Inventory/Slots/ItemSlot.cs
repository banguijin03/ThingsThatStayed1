using UnityEngine;

public delegate void ItemSlotChangeEvent(ItemSlot changedSlot);

public class ItemSlot
{
    [SerializeField] ItemContainer item;
    [SerializeField] int currentStack;

    public event ItemSlotChangeEvent OnItemSlotChanged;

    public void NoticeChanged() => OnItemSlotChanged?.Invoke(this);

    public virtual bool Containable(ItemContainer wantItem)
    {
        if (!wantItem) return false;
        if (item && item != wantItem) return false;
        if (GetIsMax()) return false;

        return true;
    }

    public ItemContainer GetItem() => item;

    public int GetStackable(ItemContainer wantItem) => wantItem ? wantItem.maxStack - currentStack : 0;

    public int GetStackable() => GetStackable(item);

    public int GetStack() => currentStack;

    public int GetHalfStack() => Mathf.CeilToInt(currentStack * 0.5f);

    public bool GetIsMax() => item ? currentStack >= item.maxStack : false;

    public bool GetIsEmpty() => !item || currentStack <= 0;

    public int Clear()
    {
        item = null;

        int removed = currentStack;
        currentStack = 0;

        return removed;
    }

    public int AddItem(ItemContainer wantItem, int amount)
    {
        if (amount <= 0) return 0;
        if (!Containable(wantItem)) return amount;

        item = wantItem;

        int stackable = Mathf.Min(item.maxStack - currentStack, amount);
        currentStack += stackable;

        return amount - stackable;
    }

    public int RemoveItem(ItemContainer wantItem)
    {
        if (!wantItem) return 0;
        if (GetIsEmpty()) return 0;
        if (item != wantItem) return 0;

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

        item = wantSlot.item;
        currentStack = wantSlot.currentStack;

        wantSlot.item = wasItem;
        wantSlot.currentStack = wasStack;
    }

    public int GiveItem(ItemSlot wantSlot) => GiveItem(wantSlot, currentStack);

    public int GiveHalfItem(ItemSlot wantSlot) => GiveItem(wantSlot, GetHalfStack());

    public int GiveSingleItem(ItemSlot wantSlot) => GiveItem(wantSlot, 1);

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

        if (InputManager.IsShift)
        {
            if (wantSlot.GetIsEmpty())
            {
                if (GetIsEmpty()) return;

                if (wantSlot.Containable(item)) GiveItem(wantSlot, Mathf.CeilToInt(currentStack * 0.5f));
            }
            else if (Containable(wantSlot.item))
            {
                wantSlot.GiveItem(this, Mathf.CeilToInt(wantSlot.currentStack * 0.5f));
            }
        }
        else
        {
            if (wantSlot.Containable(item)) GiveItem(wantSlot);
            else ExchangeItem(wantSlot);
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

        NoticeChanged();
        wantSlot.NoticeChanged();
    }
}