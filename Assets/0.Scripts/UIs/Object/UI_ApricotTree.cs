using UnityEngine;

public class UI_ApricotTree : MonoBehaviour
{
    [SerializeField] private Item_Consumable_Food apricot;
    [SerializeField] private GameObject worldItemPrefab;

    private void OnEnable()
    {
        InputManager.OnMouseLeftButton += OnMouseLeftButton;
    }

    private void OnDisable()
    {
        InputManager.OnMouseLeftButton -= OnMouseLeftButton;
    }

    private void OnMouseLeftButton(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;

        if (InputManager.CursorHoverObject != gameObject) return;

        DropApricot();
    }

    private void DropApricot()
    {
        GameObject item = Instantiate(
            worldItemPrefab,
            transform.position,
            Quaternion.identity
        );
        item.GetComponent<WorldItem>().Initialize(apricot, 1);
    }
}