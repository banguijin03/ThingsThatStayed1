using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static ItemSlot cursorSlot = new ItemSlot();

    public int columns;
    public int rows;

    ItemSlot[,] slots;

    public void Initialize()
    {
        slots = new ItemSlot[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                slots[row, column] = new ItemSlot();
            }
        }
    }

    readonly string[] itemList = {"LesserHealPotion", "CookingBook", "DesignBook", "EnerggyBook"};

    public void HealPotionPlus(int amount) 
    {
        int index = UnityEngine.Random.Range(0, itemList.Length);
        ItemContainer potion = DataManager.LoadDataFile<ItemContainer>(itemList[index]);
        AddItem(potion, amount);
    }
    public void HealPotionMinus(int amount)
    {
        ItemContainer potion = DataManager.LoadDataFile<ItemContainer>("LesserHealPotion");
        RemoveItem(potion, amount);
    }
    
    public bool IsEmpty(ItemSlot target) => target?.GetIsEmpty() ?? false;

    public void Sort(System.Comparison<ItemSlot> Method)
    {
        MergeAll();
        int totalLength = slots.Length;
        if (slots is null || slots.Length <= 1) return;
        int width = slots.GetLength(1);

        int lastFinder = totalLength - 1;
        while (lastFinder > 0)
        {
            int currentFinder = -1;
            for (int i = 0; i < lastFinder - 1; i++)
            {
                ItemSlot left = GetSlot(i, width);
                ItemSlot right = GetSlot(i + 1, width);
                int comparisonResult = Method(left, right);

                if (comparisonResult < 0)
                {
                    currentFinder = i;
                    left.ExchangeItem(right);
                }
            }
            lastFinder = currentFinder;
        }
        foreach(ItemSlot currentSlot in GetAllSlot())
        {
            currentSlot?.NoticeChanged();
        }
    }

    int ItemTypeComparison(ItemSlot left, ItemSlot right)
    {
        int result;
        if (ItemExistComparison(left, right, out result)) return result;

        ItemContainer leftItem  = left.GetItem();
        ItemContainer rightItem = right.GetItem();

        result = leftItem.CompareByType(rightItem);
        if(result!=0) return result;
        result = left.GetStack() - right.GetStack();
        return result;
    }
    int? ItemExistTypeComparison(ItemSlot left, ItemSlot right)
    {
        if (left is null) 
        {
            if (right is null) return 0;
            else return -1;
        }
        if (right is null)
        {
            return 1;
        }
        ItemContainer leftItem = left.GetItem();
        ItemContainer rightItem = right.GetItem();
        if (!leftItem)
        {
            if (!rightItem) return 0;
            else return -1;
        }
        if (!rightItem) return 1;

        return null;
    }

    bool ItemExistComparison(ItemSlot left, ItemSlot right, out int result)
    {
        int? calculated = ItemExistTypeComparison(left, right);
        result = calculated ?? 0;
        return calculated.HasValue;
    }
    public void SortByType() => Sort(ItemTypeComparison);

    public void AutoQuickInsert(Inventory other)
    {

    }
    public void AutoQuickInsert(Inventory[] other)
    {

    }

    public bool InsertAll(Inventory other)
    {
        return default;
    }
    public bool InsertAll(Inventory other, ItemContainer target)
    {
        return default;
    }

    public void LockSlot(int wantRow, int wantColumn)
    {

    }
    public void UnlockSlot(int wantRow, int wantColumn)
    {

    }

    public int CountItem(ItemContainer wantItem)
    {
        if (!wantItem) return 0;

        int result = 0;

        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            result += currentSlot.GetStack();
        }
        return result;
    }
    public int CountItem(ItemContainer wantItem, out List<ItemSlot> returnSlots)
    {
        returnSlots = new();
        if (!wantItem) return 0;
        
        int result = 0;

        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            returnSlots.Add(currentSlot);
            result += currentSlot.GetStack();
        }

        return default;
    }

    public ItemSlot GetSlot(int index, int width) => slots[index / width, index % width];
    public ItemSlot GetSlot(int index)
    {
        if (slots is null || index < 0 || slots.Length == 0 || slots.Length <= index) return null;
        int width = slots.GetLength(1);
        return slots[index / width, (index % width - 1)];
    }
    public IEnumerable<ItemSlot> GetAllSlot()
    {
        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                if (slots[row, column] is null) continue;

                yield return slots[row, column];
            }
        }
    }
    public IEnumerable<ItemSlot> GetAllSlot(System.Predicate<ItemSlot> pred)
    {
        if (pred is null) yield break;
        foreach (ItemSlot currentSlot in GetAllSlot())
        {
            if (pred(currentSlot)) yield return currentSlot;
        }
    }
    public IEnumerable<ItemSlot> GetAllSlotReverse()
    {
        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int row = height - 1; row >= 0; row--)
        {
            for (int column = width - 1; column >= 0; column--)
            {
                if (slots[row, column] is null) continue;
                yield return slots[row, column];
            }
        }
    }
    public IEnumerable<ItemSlot> GetAllSlotReverse(System.Predicate<ItemSlot> pred)
    {
        if (pred is null) yield break;
        foreach (ItemSlot currentSlot in GetAllSlotReverse())
        {
            if (pred(currentSlot)) yield return currentSlot;
        }
    }
    public IEnumerable<ItemContainer> GetAllItem()
    {
        HashSet<ItemContainer> usedItem = new();
        foreach (ItemSlot currentSlot in GetAllSlot())
        {
            ItemContainer currentItem = currentSlot.GetItem();

            if (!currentItem) continue;
            if (!usedItem.Add(currentItem)) continue;

            yield return currentItem;
        }
    }
    public Dictionary<ItemContainer, List<ItemSlot>> GetAllItemList()
    {
        Dictionary<ItemContainer, List<ItemSlot>> result = new();

        foreach(ItemSlot currentSlot in GetAllSlot())
        {
            ItemContainer currentItem = currentSlot.GetItem();
            if (!currentItem) continue;
            if(result.TryGetValue(currentItem, out List<ItemSlot> currentList))
            {
                currentList.Add(currentSlot);
            }
            else
            {
                result.Add(currentItem, new() { currentSlot });
            }
        }

        return result;
    }

    public ItemSlot FindItem(ItemContainer target)
    {

        return default;
    }
    public ItemSlot FindItem(ItemType wantType)
    {

        return default;
    }
    public ItemSlot FindItem(int wantRow, int wantColumn)
    {
        if (wantRow < 0 || wantColumn < 0) return null;
        if (wantRow >= slots.GetLength(0)) return null;
        if (wantColumn >= slots.GetLength(1)) return null;
        return slots[wantRow, wantColumn];
    }
    public ItemSlot FindItem(string containWord)
    {
        return default;
    }

    public IEnumerable<ItemSlot> FindFirstEmptySlot() => GetAllSlot(IsEmpty);
    public IEnumerable<ItemSlot> FindLastEmptySlot() => GetAllSlotReverse(IsEmpty);
    public IEnumerable<ItemSlot> FindFirstItem(ItemContainer target) => GetAllSlot((slot) => slot.GetItem() == target);
    public IEnumerable<ItemSlot> FindLastItem(ItemContainer target) => GetAllSlotReverse((slot) => slot.GetItem() == target);

    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        amount = AddItemOnExistSlots(wantItem, amount);
        if (amount <= 0) return 0;
        return AddItemOnEmptySlots(wantItem, amount);
    }
    public int AddItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            if (amount <= 0) return 0;
            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }

        return amount;
    }
    public int AddItemOnEmptySlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstEmptySlot())
        {
            if (amount <= 0) return 0;
            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }

        return amount;
    }
    public int AddItemToLocation(ItemContainer wantItem, int amount, int row, int column)
    {
        return default;
    }

    public ItemSlot[,] Clear()
    {
        ItemSlot[,] origin = slots;
        Initialize();
        return origin;
    }

    public int RemoveItem(System.Predicate<ItemSlot> condition)
    {
        return default;
    }
    public int RemoveItem(ItemContainer wantItem)
    {
        int result = 0;
        foreach(ItemSlot currentSlot in FindLastItem(wantItem))
        {
            result += currentSlot.RemoveItem(wantItem);
            currentSlot.NoticeChanged();
        }
        return result;
    }
    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        ItemSlot[] targets = FindLastItem(wantItem).ToArray();
        //Array.Sort(targets, (a, b) => a.GetStack() < b.GetStack ? 1 : 0);
        //targets.GetMinimum((current) => current.GetStack());
        foreach (ItemSlot currentSlot in FindLastItem(wantItem))
        {
            if (amount <= 0) return 0;
            amount = currentSlot.RemoveItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }
        return amount;
    }
    public int RemoveItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        return default;
    }
    public int RemoveItemFromLocation(int row, int column)
    {
        return default;
    }
    public int RemoveItemFromLocation(int row, int column, int amount)
    {
        return default;
    }

    public void MoveItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn, int amount = -1)
    {

    }

    public void MergeAll()
    {
        foreach(ItemContainer currentItem in GetAllItem())
        {
            MergeItem(currentItem);
        }
    }
    public void MergeItem(ItemContainer wantItem)
    {
        if (!wantItem) return;
        int maxStack = wantItem.maxStack;
        if (maxStack <= 1) return;
        int totalCount = CountItem(wantItem, out List<ItemSlot> containSlots);
        if (totalCount <= 1 ) return;
        if (containSlots is null) return;

        int slotCount = containSlots.Count;
        if (totalCount >= slotCount * maxStack || slotCount <= 1) return;

        int finalSlot = slotCount - 1;
        for(int i = 0; i < slotCount; i++)
        {
            ItemSlot currentSlot = containSlots[i];
            for(int j = finalSlot; j>i; j--)
            {
                if (currentSlot.GetIsMax()) break;
                ItemSlot targetSlot = containSlots[j];
                targetSlot.GiveItem(currentSlot);
                if (targetSlot.GetIsEmpty()) finalSlot--; 
            }
        }
    }


    public void ExchangeItem(int startRow, int startColumn, ItemSlot targetSlot)
    {
        if (targetSlot is null) return;
        ItemSlot first = FindItem(startRow, startColumn);
        if (first is null) return;

        first.ExchangeItem(targetSlot);
        first.NoticeChanged();
        targetSlot.NoticeChanged();
    }
    public void ExchangeItem(int startRow, int startColumn, int targetRow, int targetColumn)
    {
        ExchangeItem(startRow, startColumn, this, targetRow, targetColumn);
    }
    public void ExchangeItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn)
    {
        ItemSlot first = FindItem(startRow, startColumn);
        if (first is null) return;
        if (!targetInventory) return;
        ItemSlot second = targetInventory.FindItem(targetRow, targetColumn);
        if (second is null) return;

        first.ExchangeItem(second);
        first.NoticeChanged();
        second.NoticeChanged();
    }

    public bool UseItem(ItemContainer target)
    {
        return default;
    }
    public bool UseItem(int row, int column)
    {
        return default;
    }

}