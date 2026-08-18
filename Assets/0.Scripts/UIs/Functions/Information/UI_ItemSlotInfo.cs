using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlotInfo : UIBase
{
    [SerializeField] public Image iconImage;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] Image currentSlot;
    [SerializeField] Button button;

    [SerializeField] Sprite noneIcon;

    protected ItemSlot _connectedSlot;
    public ItemSlot ConnectedSlot => _connectedSlot;

    public event Action<UI_ItemSlotInfo> OnSlotSelected;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(SelectSlot);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(SelectSlot);
    }

    public void ConnectSlot(ItemSlot targetSlot)
    {
        DisconnectSlot();

        if (targetSlot is null) return;

        _connectedSlot = targetSlot;

        _connectedSlot.OnItemSlotChanged -= VisualUpdate;
        _connectedSlot.OnItemSlotChanged += VisualUpdate;

        VisualUpdate(_connectedSlot);
    }

    public void DisconnectSlot()
    {
        if (_connectedSlot is null) return;

        _connectedSlot.OnItemSlotChanged -= VisualUpdate;
        _connectedSlot = null;
    }

    protected virtual void VisualUpdate(ItemSlot targetSlot)
    {
        if (targetSlot is null) return;

        ItemContainer targetItem = targetSlot.GetItem();

        if (iconImage)
        {
            if (targetItem)
            {
                iconImage.sprite = targetItem.icon ?? noneIcon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }

        if (amountText)
        {
            int targetStack = targetSlot.GetStack();

            if (!targetItem || targetItem.maxStack <= 1 || targetStack <= 0)
            {
                amountText.SetText("");
            }
            else
            {
                amountText.SetText($"{targetStack}");
            }
        }
    }

    // 퀵슬롯 클릭
    public void SelectSlot()
    {
        if (_connectedSlot == null) return;

        OnSlotSelected?.Invoke(this);
    }

    // 현재 선택된 슬롯 표시
    public void SetCurrent(bool value)
    {
        if (currentSlot != null)
            currentSlot.gameObject.SetActive(value);
    }
}