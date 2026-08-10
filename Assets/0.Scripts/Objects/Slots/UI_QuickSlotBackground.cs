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

            if (currentSlot == null)
            {
                Debug.Log($"QuickSlot {i} : 슬롯 없음");
                continue;
            }

            ItemContainer item = currentSlot.GetItem();

            Debug.Log($"QuickSlot {i} : Item = {item}");

            GameObject instance = ObjectManager.CreateObject(
                itemSlotPrefabName,
                layout.transform
            );

            if (instance == null)
            {
                Debug.LogError("QuickSlot 프리팹 생성 실패");
                continue;
            }

            if (instance.TryGetComponent(out UI_ItemSlotInfo slotInfo))
            {
                slotInfo.ConnectSlot(currentSlot);
            }
            else
            {
                Debug.LogError("QuickSlot 프리팹에 UI_ItemSlotInfo가 없음");
            }
        }
    }
}