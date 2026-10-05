using System.Collections;
using UnityEngine;

public class ItemUseManager : ManagerBase
{
    UI_QuickSlotBackground quickSlot;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        quickSlot = FindAnyObjectByType<UI_QuickSlotBackground>( FindObjectsInactive.Include);

        InputManager.OnMouseLeftButton -= UseCurrentItem;
        InputManager.OnMouseLeftButton += UseCurrentItem;
        yield return null;
    }

    protected override void OnDisconnected()
    {
        InputManager.OnMouseLeftButton -= UseCurrentItem;

        quickSlot = null;
    }

    // =========================
    // 현재 슬롯 아이템 사용
    // =========================

    void UseCurrentItem(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;
        if (quickSlot == null) return;

        ItemSlot currentSlot = quickSlot.CurrentSlot;

        if (currentSlot == null ||  currentSlot.GetIsEmpty()) return;

        ItemContainer item = currentSlot.GetItem();

        if (item == null) return;

        GameObject target = InputManager.CursorHoverObject;

        if (item is Item_Consumable consumable)
        {
            UseConsumable(consumable, currentSlot, target, worldPosition);
            return;
        }

        if (item.type == ItemType.Structure)
        {
            UseStructure(item, currentSlot, worldPosition);
            return;
        }

        if (item.type == ItemType.Tool)
        {
            UseTool(item, currentSlot, target);
            return;
        }
    }

    // =========================
    // 플레이어 가져오기
    // =========================

    CharacterBase GetPlayer()
    {
        ObjectManager objectManager = FindAnyObjectByType<ObjectManager>();

        if (objectManager == null) return null;

        if (objectManager.CurrentPlayer == null) return null;

        return objectManager.CurrentPlayer.GetComponent<CharacterBase>();
    }

    // =========================
    // 소비 아이템
    // =========================

    void UseConsumable(
        Item_Consumable item,
        ItemSlot slot,
        GameObject target,
        Vector3 worldPosition)
    {
        CharacterBase player = GetPlayer();

        if (player == null) return;

        bool success = false;

        switch (item.useType)
        {
            // =========================
            // 즉시 사용
            // =========================

            case ItemUseType.Instant:

                success = item.OnUse(player, player);

                break;

            // =========================
            // 대상 사용
            // =========================

            case ItemUseType.Target:

                if (target == null) return;

                if (!target.TryGetComponent(out CharacterBase targetCharacter)) return;

                success = item.OnUse(player, targetCharacter);

                break;

            // =========================
            // 위치 사용
            // =========================

            case ItemUseType.Placement:

                success = item.OnUse(player, worldPosition);

                break;
        }

        // 사용에 성공했을 때만 1개 감소
        if (!success) return;

        slot.RemoveItem(item, 1);
        slot.NoticeChanged();
    }

    // =========================
    // 구조물
    // =========================

    void UseStructure(
        ItemContainer item,
        ItemSlot slot,
        Vector3 worldPosition)
    {
        // TODO
        // 상자 / 설치물 구현할 때 작성
    }

    // =========================
    // 도구
    // =========================

    void UseTool(
        ItemContainer item,
        ItemSlot slot,
        GameObject target)
    {
        // TODO
        // 도구 사용 구현할 때 작성
    }
}