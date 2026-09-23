using UnityEngine;

public class UI_ApricotTree : MonoBehaviour
{
    [SerializeField] private Item_Consumable_Food apricot;
    [SerializeField] private GameObject worldItemPrefab;
    [SerializeField] private InteractionCondition interactionCondition;

    private void OnEnable()
    {
        InputManager.OnMouseLeftButton -= OnMouseLeftButton;
        InputManager.OnMouseLeftButton += OnMouseLeftButton;
    }

    private void OnDisable()
    {
        InputManager.OnMouseLeftButton -= OnMouseLeftButton;
    }

    private void OnMouseLeftButton(
        bool value,
        Vector2 screenPosition,
        Vector3 worldPosition)
    {
        if (!value)
            return;

        Vector3 clickWorld = worldPosition;
        clickWorld.z = 0;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite == null)
            return;

        if (!sprite.bounds.Contains(clickWorld))
            return;

        if (interactionCondition == null)
            return;

        if (!interactionCondition.CanInteract())
            return;

        DropApricot();
    }

    private void DropApricot()
    {
        if (worldItemPrefab == null)
            return;

        if (apricot == null)
            return;

        GameObject item = Instantiate(
            worldItemPrefab,
            transform.position,
            Quaternion.identity
        );

        WorldItem worldItem = item.GetComponent<WorldItem>();

        if (worldItem == null)
            return;

        worldItem.Initialize(apricot, 198);
    }
}