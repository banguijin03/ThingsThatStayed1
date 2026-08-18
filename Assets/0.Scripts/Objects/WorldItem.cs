using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] float pickupRange = 1.5f;
    [SerializeField] float moveSpeed = 5f;

    private ItemContainer itemData;
    private int amount;

    private Inventory targetInventory;
    private PlayerController player;
    private Collider2D playerCollider;

    private bool isMoving;

    private void Awake()
    {
        targetInventory = FindAnyObjectByType<Inventory>();
        player = FindAnyObjectByType<PlayerController>();

        if (player != null)
            playerCollider = player.GetComponent<Collider2D>();
    }

    public void Initialize(ItemContainer data, int count)
    {
        itemData = data;
        amount = count;
    }

    private void Update()
    {
        if (playerCollider == null) return;

        Vector2 playerPosition = playerCollider.bounds.center;

        float distance = Vector2.Distance(
            transform.position,
            playerPosition
        );

        if (distance <= pickupRange)
        {
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isMoving) return;

        if (other != playerCollider) return;

        if (targetInventory == null) return;

        targetInventory.AddItem(itemData, amount);

        Destroy(gameObject);
    }
}