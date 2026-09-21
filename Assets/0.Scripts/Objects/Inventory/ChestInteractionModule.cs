using UnityEngine;

public class ChestInteractionModule : MonoBehaviour
{
    [SerializeField] InteractionCondition interactionCondition;
    [SerializeField] Inventory chestInventory;

    bool interactionLock;

    private void Awake()
    {
        if (interactionCondition == null)
            interactionCondition = GetComponent<InteractionCondition>();

        if (chestInventory == null)
            chestInventory = GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        InputManager.OnInteraction -= InteractionChest;
        InputManager.OnInteraction += InteractionChest;
    }

    private void OnDisable()
    {
        InputManager.OnInteraction -= InteractionChest;
    }

    public void InteractionChest(bool value)
    {
        if (!value)
        {
            interactionLock = false;
            return;
        }

        if (interactionLock)
            return;

        if (interactionCondition == null)
            return;

        if (!interactionCondition.CanInteract())
            return;

        if (GameManager.Instance.Dialogue.IsDialogue)
            return;

        if (chestInventory == null)
            return;

        interactionLock = true;

        UIBase ui = UIManager.ClaimGetUI(UIType.Chest);

        if (ui is UI_ChestWindow chestWindow)
        {
            if (chestWindow.IsOpen)
            {
                UIManager.ClaimCloseUI(UIType.Chest);
                return;
            }

            chestWindow.ConnectChest(chestInventory);
        }

        UIManager.ClaimOpenUI(UIType.Chest);
    }
}