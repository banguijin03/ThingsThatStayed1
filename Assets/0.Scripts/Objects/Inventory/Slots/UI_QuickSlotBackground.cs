using UnityEngine;
using UnityEngine.UI;

public class UI_QuickSlotBackground : UIBase
{
    [SerializeField] LayoutGroup layout;
    [SerializeField] string itemSlotPrefabName;
    Inventory inventory;
    ItemSlot currentStack;

    private ItemSlot currentSlot;
    public ItemSlot CurrentSlot => currentSlot;

    public ItemContainer CurrentItem =>
        currentSlot != null ? currentSlot.GetItem() : null;

    private UI_ItemSlotInfo[] slotInfos;
    private int currentIndex = 0;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        inventory = FindAnyObjectByType<Inventory>(FindObjectsInactive.Include);

        if (inventory != null)
        {
            ConnectInventory();
            SelectSlot(0);
        }

        InputManager.OnMouseWheel -= MouseWheel;
        InputManager.OnMouseWheel += MouseWheel;

        InputManager.OnMouseLeftButton -= MouseLeftClick;
        InputManager.OnMouseLeftButton += MouseLeftClick;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);

        InputManager.OnMouseWheel -= MouseWheel;
        InputManager.OnMouseLeftButton -= MouseLeftClick;
    }

    void ConnectInventory()
    {
        slotInfos = new UI_ItemSlotInfo[inventory.columns];

        for (int i = 0; i < inventory.columns; i++)
        {
            ItemSlot slot = inventory.GetSlot(i);

            if (slot == null)
                continue;

            GameObject instance = ObjectManager.CreateObject(
                itemSlotPrefabName,
                layout.transform
            );

            if (instance == null)
                continue;

            if (instance.TryGetComponent(out UI_ItemSlotInfo slotInfo))
            {
                slotInfo.ConnectSlot(slot);

                int index = i;

                slotInfo.OnSlotSelected -= SelectSlotFromUI;
                slotInfo.OnSlotSelected += SelectSlotFromUI;

                slotInfos[index] = slotInfo;
            }
        }
    }

    void SelectSlotFromUI(UI_ItemSlotInfo slotInfo)
    {
        for (int i = 0; i < slotInfos.Length; i++)
        {
            if (slotInfos[i] == slotInfo)
            {
                SelectSlot(i);
                return;
            }
        }
    }

    void SelectSlot(int index)
    {
        if (slotInfos == null || slotInfos.Length == 0)
            return;

        if (index < 0)
            index = slotInfos.Length - 1;

        if (index >= slotInfos.Length)
            index = 0;

        if (slotInfos[index] == null)
            return;

        currentIndex = index;
        currentSlot = inventory.GetSlot(index);

        for (int i = 0; i < slotInfos.Length; i++)
        {
            if (slotInfos[i] != null)
                slotInfos[i].SetCurrent(i == currentIndex);
        }
    }
    void MouseLeftClick(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;
        if (CurrentItem == null)  return;
        GameObject target = InputManager.CursorHoverObject;
        if (target == null) return;
        CharacterBase character = target.GetComponentInParent<CharacterBase>();
        if (character == null) return;
        if (CurrentItem is Item_Consumable_Food food) food.OnUse(null, character); 
    }

    void MouseWheel(float value)
    {
            if (value > 0) SelectSlot(currentIndex - 1);
            else if (value < 0) SelectSlot(currentIndex + 1);
    }
}