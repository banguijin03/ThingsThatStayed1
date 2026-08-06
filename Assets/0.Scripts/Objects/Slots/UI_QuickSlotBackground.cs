using UnityEngine;
using UnityEngine.UI;

public class UI_QuickSlotBackground : UIBase
{
    [SerializeField] LayoutGroup layout;
    [SerializeField] string itemSlotPrefabName;
    Inventory inventory;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        inventory = FindAnyObjectByType<Inventory>(FindObjectsInactive.Include);

        if (inventory != null)
        {
            ConnectInventory();
        }
    }

    void ConnectInventory()
    {
        for (int i = 0; i < inventory.columns; i++)
        {
            ItemSlot currentSlot = inventory.GetSlot(i);

            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, layout.transform);

            if (instance.TryGetComponent(out UI_ItemSlotInfo slotInfo))
            {
                slotInfo.ConnectSlot(currentSlot);
            }
        }
    }
}