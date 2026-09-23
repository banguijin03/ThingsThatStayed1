using UnityEngine;
using UnityEngine.UI;

public class UI_ChestWindow : UI_ScreenBase
{
    [SerializeField] LayoutGroup inventoryLayout;
    [SerializeField] LayoutGroup chestLayout;
    [SerializeField] string itemSlotPrefabName;

    Inventory playerInventory;
    Inventory chestInventory;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        InputManager.OnCancel -= OnCancel;
        InputManager.OnCancel += OnCancel;

        PlayerController player = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (player == null) return;

        playerInventory = player.GetComponent<Inventory>();
        if (playerInventory == null) return;

        ConnectPlayerInventory();
    }

    public override void Unregistration(UIManager manager)
    {
        InputManager.OnCancel -= OnCancel;

        DisconnectInventory();
        DisconnectChest();

        playerInventory = null;
        chestInventory = null;

        base.Unregistration(manager);
    }

    void OnCancel(bool value)
    {
        if (!value) return;
        if (!IsOpen) return;
        if (!CloseByCancel) return;

        UIManager.ClaimCloseUI(UIType.Chest);
    }

    void ConnectPlayerInventory()
    {
        if (playerInventory == null) return;
        if (inventoryLayout == null) return;

        DisconnectInventory();

        if (inventoryLayout is GridLayoutGroup grid)
            grid.constraintCount = playerInventory.columns;

        foreach (ItemSlot slot in playerInventory.GetAllSlot())
        {
            if (slot == null) continue;

            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, inventoryLayout.transform);
            if (instance == null) continue;

            if (instance.TryGetComponent(out UI_ItemSlotInfo slotInfo))
                slotInfo.ConnectSlot(slot);
        }
    }

    public void ConnectChest(Inventory inventory)
    {
        if (inventory == null) return;
        if (chestLayout == null) return;

        DisconnectChest();

        chestInventory = inventory;

        if (chestLayout is GridLayoutGroup grid)
            grid.constraintCount = chestInventory.columns;

        foreach (ItemSlot slot in chestInventory.GetAllSlot())
        {
            if (slot == null) continue;

            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, chestLayout.transform);
            if (instance == null) continue;

            if (instance.TryGetComponent(out UI_ItemSlotInfo slotInfo))
                slotInfo.ConnectSlot(slot);
        }
    }

    void DisconnectInventory()
    {
        if (inventoryLayout == null) return;

        while (inventoryLayout.transform.childCount > 0)
        {
            Transform child = inventoryLayout.transform.GetChild(0);
            child.SetParent(null);
            ObjectManager.DestroyObject(child.gameObject);
        }
    }

    void DisconnectChest()
    {
        if (chestLayout == null) return;

        while (chestLayout.transform.childCount > 0)
        {
            Transform child = chestLayout.transform.GetChild(0);
            child.SetParent(null);
            ObjectManager.DestroyObject(child.gameObject);
        }
    }

    public void CloseWindow()
    {
        UIManager.ClaimCloseUI(UIType.Chest);
    }
}