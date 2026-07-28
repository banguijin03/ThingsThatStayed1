using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : UIBase
{
    [SerializeField] LayoutGroup layout;
    [SerializeField] string itemSlotPrefabName;

    Inventory inventory;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        inventory = FindAnyObjectByType<Inventory>(FindObjectsInactive.Include);

        Debug.Log($"Inventory : {inventory}");

        if (inventory != null)
        {
            Debug.Log("ConnectInventory 호출");
            ConnectInventory();
        }
    }

    void ConnectInventory()
    {
        Debug.Log($"columns = {inventory.columns}");

        for (int i = 0; i < inventory.columns; i++)
        {
            Debug.Log($"슬롯 {i}");

            ItemSlot currentSlot = inventory.GetSlot(i);

            Debug.Log(currentSlot);

            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, layout.transform);
            Debug.Log(instance);

            if (instance.TryGetComponent(out UI_ItemSlotInfo slotInfo))
            {
                Debug.Log("ConnectSlot");
                slotInfo.ConnectSlot(currentSlot);
            }
        }
    }
}